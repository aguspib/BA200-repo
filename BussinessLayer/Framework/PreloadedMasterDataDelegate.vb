Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.IO
Imports Biosystems.Ax00.BL.Framework

Namespace Biosystems.Ax00.BL

    Public Class PreloadedMasterDataDelegate

        Private appSession As ApplicationSessionManager
        Private iconPath As String
        Private resultData As GlobalDataTO

#Region "Methods"

        Public Sub New()
            'Get full path for images
            appSession = New ApplicationSessionManager()
            iconPath = GlobalBase.GetSessionInfo.ApplicationIconPath
        End Sub

        ''' <summary>
        ''' Get the list of all values of the specified Sub Table in the group of Preloaded Master Data Sub Tables 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>
        ''' created by: DL 28/03/2011
        ''' </remarks>
        Public Function GetLegendList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As String) As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim PreloadedMasterData As New tfmwPreloadedMasterDataDAO
                        resultData = PreloadedMasterData.ReadLegendBySubTableID(dbConnection, pSubTableID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreloadedMasterDataDelegate.GetLegendList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the list of all values of the specified Sub Table in the group of Preloaded Master Data Sub Tables 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>
        ''' Modified AG 21/02/2014 - #1516</remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As PreloadedMasterDataEnum) As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim PreloadedMasterData As New tfmwPreloadedMasterDataDAO
                        'AG 21/02/2014 - #1516
                        'resultData = PreloadedMasterData.ReadBySubTableID(dbConnection, pSubTableID)
                        If pSubTableID <> PreloadedMasterDataEnum.ICON_PATHS Then
                            resultData = PreloadedMasterData.ReadBySubTableID(dbConnection, pSubTableID)
                        Else
                            resultData = PreloadedMasterData.ReadBySubTableIDWithOutLanguages(dbConnection, pSubTableID)
                        End If
                        'AG 21/02/2014 - #1516

                        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                            Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                            If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreloadedMasterDataDelegate.GetList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get value of an specific Item of a Preloaded Master Data Sub Table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pItemID">Unique identifier of the specific Item which value will be obtained</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PreloadedMasterDataDS with the data of the specified
        '''          subtable Item</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2010 - As copy of the existing function (same name) but following the current template
        ''' AG 21/02/2014 - #1516 sql memory usage improvements. Queries for ICON_PATHS has a special query with no INNER JOIN with multilanguage
        ''' </remarks>
        Public Function GetSubTableItem(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As PreloadedMasterDataEnum, _
                                       ByVal pItemID As String) As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPreloadedMasterDataDAO As New tfmwPreloadedMasterDataDAO

                        'AG 21/02/2014 - #1516
                        'resultData = myPreloadedMasterDataDAO.ReadByItemID(dbConnection, pSubTableID, pItemID)
                        If pSubTableID <> PreloadedMasterDataEnum.ICON_PATHS Then
                            resultData = myPreloadedMasterDataDAO.ReadByItemID(dbConnection, pSubTableID, pItemID)
                        Else
                            resultData = myPreloadedMasterDataDAO.ReadByItemIDWithOutLanguages(dbConnection, pSubTableID, pItemID)
                        End If
                        'AG 21/02/2014 - #1516

                        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                            Dim resultDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                            If (resultDS.tfmwPreloadedMasterData.Rows.Count = 0) Then
                                'AG 06/09/2011 - Service Patch 2011 (adding dilsolutions to worksession ... the tube content is saved as specsolution not as dilsolutions)
                                Dim errorFlag As Boolean = False
                                If (pSubTableID = GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS) Then
                                    resultData = myPreloadedMasterDataDAO.ReadByItemID(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS, pItemID)
                                    If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                                        resultDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (resultDS.tfmwPreloadedMasterData.Rows.Count = 0) Then errorFlag = True
                                    End If
                                Else
                                    errorFlag = True
                                End If

                                If (errorFlag) Then
                                    resultData.HasError = True
                                    resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                                End If
                                'AG 06/09/2011
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreloadedMasterDataDelegate.GetSubTableItem", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the Icon for the specified element, searchs the image full path from the AppSettings, and returns
        ''' the Icon converted into a byte array
        ''' </summary>
        ''' <returns>Array of bytes containing the Icon</returns>
        ''' <remarks>
        ''' Created by: GDS 19/02/2010
        ''' Modified by: RH 08/23/2010
        '''              RH 14/06/2011
        '''              TR 18/02/2013 - Added the optional parameter pDBConnection to be used on the update process
        '''              SA 18/02/2014 - Added the Finally section to close the DB Connection when it has been opened locally
        ''' </remarks>
        Public Function GetIconImage(ByVal pImageName As String, Optional ByVal pDBConnection As SqlClient.SqlConnection = Nothing) As Byte()
            Dim bArrImage() As Byte = Nothing
            Dim iconFullPath As String = String.Empty
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        resultData = Me.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, pImageName)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim preloadedDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                            If (preloadedDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                iconFullPath = iconPath + preloadedDataDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                'Convert the Icon Image to a byte array
                                If (IO.File.Exists(iconFullPath)) Then
                                    Using fsImage As FileStream = System.IO.File.OpenRead(iconFullPath)
                                        bArrImage = New Byte(CInt(fsImage.Length)) {}
                                        fsImage.Read(bArrImage, 0, Convert.ToInt32(fsImage.Length))
                                        fsImage.Flush()
                                        fsImage.Close()
                                    End Using
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PreloadedMasterDataDelegate.GetIconImage", EventLogEntryType.Error, False)
                Throw
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return bArrImage
        End Function
#End Region

#Region "TO REVIEW - DELETE"
        ''' <summary>
        ''' Get value of an specif Item in the specified Preloade Master Data Sub Table
        ''' </summary>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pItemID">Unique identifier of the specific Item which value will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks></remarks>
        Public Function GetSubTableItem(ByVal pSubTableID As String, _
                                        ByVal pItemID As String) As PreloadedMasterDataDS
            Dim resultDS As New PreloadedMasterDataDS

            Try
                Dim resultData As New tfmwPreloadedMasterDataDAO
                resultDS = resultData.ReadByItemID(pSubTableID, pItemID)

            Catch ex As Exception
                Throw ex
            End Try

            Return resultDS

        End Function
#End Region

    End Class

End Namespace