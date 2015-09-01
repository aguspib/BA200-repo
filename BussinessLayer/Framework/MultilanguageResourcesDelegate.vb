Option Strict On
Option Explicit On

Imports System.Collections.Concurrent
Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.Interfaces

Namespace Biosystems.Ax00.BL

    Public Class MultilanguageResourcesDelegate
        Implements ITextParser

        'RH 02/05/2011
        Private Shared MultiLanguageResources As MultiLanguageDS = Nothing
        Private Shared CurrentLanguage As String = "ENG"


        Public Shared mlResourceDictionary As New ConcurrentDictionary(Of String, Func(Of String))

        Const CSTART = "["c
        Const CEND = "]"c

        'RegularText / PosibleKeywordStart / IdentifierText
        Private Enum ParserStatus
            RegularText
            PosibleKeywordStart
            IdentifierText
        End Enum

        Public Sub New()
            'RegisterKeyword("MODEL", Function() "BA200")
            If GlobalBase.TextParser Is Nothing Then GlobalBase.TextParser = Me
        End Sub

#Region "Parser Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="address"></param>
        ''' <remarks></remarks>
        Public Shared Sub RegisterKeyword(key As String, address As Func(Of String))

            If key IsNot Nothing And Not mlResourceDictionary.ContainsKey(key) Then

                Dim cont As Int32 = 0
                While Not mlResourceDictionary.TryAdd(key.ToUpperInvariant(), address)
                    If cont <= 3 Then
                        CreateLogActivity("The Pair of Key/value parameters can't be added to this dictionary collection.", EventLogEntryType.Error)
                        Exit While
                    End If
                    cont = CInt(cont + 1)
                End While

            End If
        End Sub

        Public Function ParseKeywords(originalText As String) As String
            Return ParseKeywords(originalText, True)
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="originalText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ParseKeywords(originalText As String, logOnError As Boolean) As String Implements ITextParser.ParseText
            Dim getKeyValue As String
            Dim oword As New StringBuilder, strResult As New StringBuilder(1024)
            Dim isChardEnd As Boolean = False
            Dim lastChar As Boolean = False
            Dim conIndex As Integer = 0

            'RegularText / PosibleKeywordStart / IdentifierText
            Dim statusText As ParserStatus = ParserStatus.RegularText

            'Try

            If originalText IsNot Nothing Then
                If originalText.Contains(CSTART) Then
                    For Each c As Char In originalText

                        conIndex += 1
                        If conIndex = originalText.Length Then
                            lastChar = True
                        End If

                        Select Case (statusText)
                            Case ParserStatus.RegularText

                                If c = CSTART Then statusText = ParserStatus.PosibleKeywordStart Else strResult.Append(c)

                            Case ParserStatus.PosibleKeywordStart
                                If c = CSTART Then
                                    If lastChar Then
                                        If logOnError Then CreateLogActivity("The following text:  " & originalText & " has a sintaxis error.", EventLogEntryType.Information)
                                        strResult.Append("[[")
                                    Else
                                        statusText = ParserStatus.IdentifierText
                                    End If
                                Else
                                    strResult.Append("[" & c)
                                    statusText = ParserStatus.RegularText
                                End If

                            Case ParserStatus.IdentifierText
                                If c = CEND Then
                                    If Not isChardEnd Then
                                        If lastChar Then
                                            'CASE [[asdjkhgdf]. --> end
                                            If logOnError Then CreateLogActivity("The following text:  " & originalText & " has a sintaxis error.", EventLogEntryType.Information)
                                            strResult.Append(oword)
                                        Else
                                            isChardEnd = True
                                        End If
                                    Else
                                        'second ]
                                        If oword.Length > 0 AndAlso Not oword.Length.ToString.Equals(" ") Then
                                            If mlResourceDictionary.ContainsKey(oword.ToString().ToUpper()) Then
                                                getKeyValue = mlResourceDictionary(oword.ToString().ToUpper())()
                                                strResult.Append(getKeyValue)
                                            Else
                                                'CASE Key not in Dictionary
                                                If logOnError Then CreateLogActivity("The Key word of this Text: " & originalText & " is not registered in the reserved dictionary.", EventLogEntryType.Error)
                                                strResult.Append(oword)
                                            End If
                                        Else
                                            'CASE [[]] or [[ ]]
                                            If logOnError Then CreateLogActivity("The following text:  " & originalText & " has a sintaxis error.", EventLogEntryType.Information)
                                            strResult.Append(oword)
                                        End If
                                        isChardEnd = False
                                        oword.Clear()
                                        statusText = ParserStatus.RegularText
                                    End If
                                Else
                                    'oword.Append(Char.ToUpper(c))
                                    oword.Append(c)
                                End If
                        End Select

                    Next
                Else
                    strResult.Append(originalText)
                End If
            End If
            Return strResult.ToString()


        End Function

#End Region

#Region "Methods"

        ''' <summary>
        ''' Gets the resource text related to the specified Resource and Language. Just to be sure a text is 
        ''' returned, if it was not found for the specified Language, the English Text is searched and returned.
        ''' </summary>
        ''' <param name="pResource">Code of the Resource for which the resource value is searched</param>
        ''' <returns>String containing the ResourceText</returns>
        ''' <remarks>
        ''' Created by: RH 22/02/2012 New optimized version.
        '''             Optimization: Shared Function. No need to pass CurrentLanguage. No need to pass pDBConnection.
        ''' AG 18/02/2014 -#1505 - release linq memory
        ''' </remarks>
        Public Shared Function GetResourceText(ByVal pResource As String) As String
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim myMultiLanguageDAO As tfmwMultilanguageResourcesDAO = Nothing

            Try
                LoadCacheOfResourcesDataIfItIsEmpty()

                If MultiLanguageResources Is Nothing Then 'There is no cache
                    Return String.Empty
                End If

                'Here the cache has data

                Dim Resource As List(Of MultiLanguageDS.tfmwMultiLanguageResourcesRow)

                Resource = (From row In MultiLanguageResources.tfmwMultiLanguageResources _
                            Where row.ResourceID = pResource _
                            Select row).ToList()

                If Resource.Count > 0 Then
                    Return Resource(0).ResourceText 'The resource we are looking for in the Current Language
                Else
                    'Resource in Current Language not Found
                    'Search for the English version

                    'Open DB connection
                    If dbConnection Is Nothing Then
                        myGlobalDataTO = DAOBase.GetOpenDBConnection(Nothing)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                        Else
                            'Dim myLogAcciones As New ApplicationLogManager()
                            GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                            Return String.Empty
                        End If
                    End If

                    'Here we have an open connection

                    If myMultiLanguageDAO Is Nothing Then
                        myMultiLanguageDAO = New tfmwMultilanguageResourcesDAO()
                    End If

                    'Search for the English version
                    myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, pResource, CurrentLanguage)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myMultiLanguageDS As MultiLanguageDS
                        myMultiLanguageDS = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)

                        If (myMultiLanguageDS.tfmwMultiLanguageResources.Rows.Count = 1) Then
                            Return myMultiLanguageDS.tfmwMultiLanguageResources(0).ResourceText 'The Resource, English version
                        Else
                            Return String.Empty
                        End If
                    Else
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                        Return String.Empty
                    End If
                End If
                Resource = Nothing 'AG 18/02/2014 -#1505 - release linq memory

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                Throw

            Finally
                If Not dbConnection Is Nothing Then dbConnection.Close()

            End Try

            Return String.Empty
        End Function

        ''' <summary>
        ''' Loads cache of resources data if it is empty
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH 22/02/2012
        ''' </remarks>
        Private Shared Sub LoadCacheOfResourcesDataIfItIsEmpty()
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim myMultiLanguageDAO As tfmwMultilanguageResourcesDAO = Nothing

            'Load cache of resources data if it is empty
            If MultiLanguageResources Is Nothing Then 'There is no cache
                'Open DB connection
                myGlobalDataTO = DAOBase.GetOpenDBConnection(Nothing)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        myMultiLanguageDAO = New tfmwMultilanguageResourcesDAO()

                        myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, CurrentLanguage)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            MultiLanguageResources = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)
                        Else
                            'Dim myLogAcciones As New ApplicationLogManager()
                            GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                        End If
                    Else
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                    End If
                Else
                    'Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Updates the Application Current Language
        ''' </summary>
        ''' <param name="Language">The new Language</param>
        ''' <remarks>
        ''' Created by: RH 22/02/2012
        ''' </remarks>
        Public Shared Sub SetCurrentLanguage(ByVal Language As String)
            Try
                If Not String.IsNullOrEmpty(Language) Then
                    CurrentLanguage = Language
                    MultiLanguageResources = Nothing
                    LoadCacheOfResourcesDataIfItIsEmpty()
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MultilanguageResourcesDelegate.SetCurrentLanguage", EventLogEntryType.Error, False)
                Throw

            End Try
        End Sub

        ''' <summary>
        ''' Retrieves the Application Current Language
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH 01/03/2012
        ''' </remarks>
        Public Shared Function GetCurrentLanguage() As String
            Return CurrentLanguage
        End Function

        'Old version. To be deleted.
        ''' <summary>
        ''' Get the resource text related to the specified Resource and Language. Just to be sure a text is 
        ''' returned, if it was not found for the specified Language, the English Text is searched and returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResource">Code of the Resource for which the resource value is searched</param>
        ''' <param name="pLanguage">Code of the Language for which the resource is searched</param>
        ''' <returns>String containing the ResourceText</returns>
        ''' <remarks>
        ''' Created by: RH 21/09/2011 New optimized version
        '''             Optimization: Only open a DB connection if it is needed
        ''' </remarks>
        Public Function GetResourceText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResource As String, ByVal pLanguage As String) As String
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim myMultiLanguageDAO As tfmwMultilanguageResourcesDAO = Nothing

            Try
                'Load cache of resources data if it is empty or the Current Language has changed
                If CurrentLanguage <> pLanguage OrElse MultiLanguageResources Is Nothing Then 'There is no cache
                    'Open DB connection
                    myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                        If (Not dbConnection Is Nothing) Then
                            myMultiLanguageDAO = New tfmwMultilanguageResourcesDAO()

                            'CurrentLanguage = pLanguage 'RH Commented 22/02/2012
                            myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, pLanguage)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                MultiLanguageResources = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)
                            Else
                                'Dim myLogAcciones As New ApplicationLogManager()
                                GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                                Return String.Empty
                            End If
                        Else
                            'Dim myLogAcciones As New ApplicationLogManager()
                            GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                            Return String.Empty
                        End If
                    Else
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                        Return String.Empty
                    End If
                End If

                'Here the cache has data

                Dim Resource As List(Of MultiLanguageDS.tfmwMultiLanguageResourcesRow)

                Resource = (From row In MultiLanguageResources.tfmwMultiLanguageResources _
                            Where row.ResourceID = pResource _
                            Select row).ToList()

                If Resource.Count > 0 Then
                    Return Resource(0).ResourceText 'The resource we are looking for in the Current Language
                Else
                    'Resource in Current Language not Found
                    'Search for the English version

                    'Open DB connection
                    If dbConnection Is Nothing Then
                        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                        Else
                            'Dim myLogAcciones As New ApplicationLogManager()
                            GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                            Return String.Empty
                        End If
                    End If

                    'If dbConnection Is Nothing Then
                    '    'Dim myLogAcciones As New ApplicationLogManager()
                    '    GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                    '    Return String.Empty
                    'End If

                    'Here we have an open connection

                    If myMultiLanguageDAO Is Nothing Then
                        myMultiLanguageDAO = New tfmwMultilanguageResourcesDAO()
                    End If

                    'Search for the English version
                    myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, pResource, pLanguage)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myMultiLanguageDS As MultiLanguageDS
                        myMultiLanguageDS = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)

                        If (myMultiLanguageDS.tfmwMultiLanguageResources.Rows.Count = 1) Then
                            Return myMultiLanguageDS.tfmwMultiLanguageResources(0).ResourceText 'The Resource, English version
                        Else
                            Return String.Empty
                        End If
                    Else
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                        Return String.Empty
                    End If
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
                Throw

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return String.Empty
        End Function

        '''' <summary>
        '''' Get the resource text related to the specified Resource and Language. Just to be sure a text is 
        '''' returned, if it was not found for the specified Language, the English Text is searched and returned
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pResource">Code of the Resource for which the resource value is searched</param>
        '''' <param name="pLanguage">Code of the Language for which the resource is searched</param>
        '''' <returns>String containing the ResourceText</returns>
        '''' <remarks>
        '''' Created by: RH 02/05/2011
        '''' </remarks>
        'Public Function GetResourceText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResource As String, ByVal pLanguage As String) As String
        '    Dim myGlobalDataTO As GlobalDataTO
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myMultiLanguageDAO As New tfmwMultilanguageResourcesDAO

        '                'Load cache of resources data if needed
        '                If CurrentLanguage <> pLanguage OrElse MultiLanguageResources Is Nothing Then
        '                    CurrentLanguage = pLanguage
        '                    myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, pLanguage)
        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        MultiLanguageResources = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)
        '                    Else
        '                        'Dim myLogAcciones As New ApplicationLogManager()
        '                        GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
        '                        Return String.Empty
        '                    End If
        '                End If

        '                Dim Resource As List(Of MultiLanguageDS.tfmwMultiLanguageResourcesRow)
        '                Resource = (From row In MultiLanguageResources.tfmwMultiLanguageResources _
        '                            Where row.ResourceID = pResource _
        '                            Select row).ToList()

        '                If Resource.Count > 0 Then
        '                    Return Resource(0).ResourceText
        '                Else
        '                    'Resource in Current Language not Found
        '                    'Search for the English version
        '                    myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, pResource, pLanguage)
        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim myMultiLanguageDS As MultiLanguageDS
        '                        myMultiLanguageDS = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)

        '                        If (myMultiLanguageDS.tfmwMultiLanguageResources.Rows.Count = 1) Then
        '                            Return myMultiLanguageDS.tfmwMultiLanguageResources(0).ResourceText
        '                        Else
        '                            Return String.Empty
        '                        End If
        '                    Else
        '                        'Dim myLogAcciones As New ApplicationLogManager()
        '                        GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
        '                        Return String.Empty
        '                    End If
        '                End If
        '            End If
        '        Else
        '            'Dim myLogAcciones As New ApplicationLogManager()
        '            GlobalBase.CreateLogActivity(myGlobalDataTO.ErrorMessage, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
        '            Return String.Empty
        '        End If

        '    Catch ex As Exception
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)
        '        Throw

        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return String.Empty
        'End Function

        '''' <summary>
        '''' Get the resource text related to the specified Resource and Language. Just to be sure a text is 
        '''' returned, if it was not found for the specified Language, the English Text is searched and returned
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pResource">Code of the Resource for which the resource value is searched</param>
        '''' <param name="pLanguage">Code of the Language for which the resource is searched</param>
        '''' <returns>String containing the ResourceText</returns>
        '''' <remarks>
        '''' Created by: PG 05/10/2010 
        '''' </remarks>
        'Public Function GetResourceText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResource As String, ByVal pLanguage As String) As String
        '    Dim textLanguage As String = ""
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim stopSearching As Boolean = False
        '                Dim myMultiLanguageDAO As New tfmwMultilanguageResourcesDAO

        '                Do While (Not stopSearching)
        '                    myGlobalDataTO = myMultiLanguageDAO.Read(dbConnection, pResource, pLanguage)
        '                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        'Dim myMultiLanguageDS As New MultiLanguageDS
        '                        Dim myMultiLanguageDS As MultiLanguageDS 'RH 20/10/2010 Remove 'New', because creates a new object just to be trashed in the next line
        '                        myMultiLanguageDS = DirectCast(myGlobalDataTO.SetDatos, MultiLanguageDS)

        '                        If (myMultiLanguageDS.tfmwMultiLanguageResources.Rows.Count = 1) Then
        '                            textLanguage = myMultiLanguageDS.tfmwMultiLanguageResources(0).ResourceText
        '                            stopSearching = True
        '                        Else
        '                            If (pLanguage <> "ENG") Then
        '                                pLanguage = "ENG"
        '                            Else
        '                                stopSearching = True
        '                            End If
        '                        End If
        '                    Else
        '                        stopSearching = True
        '                    End If
        '                Loop
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "MultilanguageResourcesDelegate.GetResourceText", EventLogEntryType.Error, False)

        '        'Send the exception to the method that called this (due to in this case it does not return a GlobalDataTO)
        '        'Throw ex  'Commented line RH 23/12/2010
        '        'Do prefer using an empty throw when catching and re-throwing an exception.
        '        'This is the best way to preserve the exception call stack.
        '        'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
        '        'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
        '        Throw

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return textLanguage
        'End Function

#End Region

    End Class

End Namespace

