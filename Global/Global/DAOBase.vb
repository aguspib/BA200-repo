Option Explicit On
Option Strict On




Namespace Biosystems.Ax00.Global

    Public Module DAOBase


        Private DBServerField As String = String.Empty
        Private CurrentDBField As String = String.Empty
        Private ConnectionString As String = String.Empty

        'RH 17/05/2011
        Private DBLoginField As String = String.Empty
        Private DBPasswordField As String = String.Empty

        Public ReadOnly Property DBServer() As String
            Get
                If String.IsNullOrEmpty(DBServerField) Then GetConnectionString()
                Return DBServerField
            End Get
        End Property

        Public ReadOnly Property CurrentDB() As String
            Get
                If String.IsNullOrEmpty(CurrentDBField) Then GetConnectionString()
                Return CurrentDBField
            End Get
        End Property

        'RH 17/05/2011
        Public ReadOnly Property DBLogin() As String
            Get
                If String.IsNullOrEmpty(DBLoginField) Then GetConnectionString()
                Return DBLoginField
            End Get
        End Property

        'RH 17/05/2011
        Public ReadOnly Property DBPassword() As String
            Get
                If String.IsNullOrEmpty(DBPasswordField) Then GetConnectionString()
                Return DBPasswordField
            End Get
        End Property

        ''' <summary>
        ''' Read the encripted Connection string from App.Config and return it in plain text
        ''' Updates the CurrentDBServerName and CurrentDBName values
        ''' </summary>
        ''' <returns>Connection String</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: RH 16/11/2010
        ''' Modified by: RH 17/05/2011 Add DB Login and Password.
        ''' </remarks>
        Public Function GetConnectionString(Optional ByVal pUpdateConnection As Boolean = False) As String
            Try
                'Dim mySecurity As New Security.Security
                'myConnectionString = mySecurity.Decryption(ConfigurationManager.ConnectionStrings("BiosystemsConn").ConnectionString)

                If Not String.IsNullOrEmpty(ConnectionString) Then Return ConnectionString

                'CurrentConnectionString = ConfigurationManager.ConnectionStrings("BiosystemsConn").ConnectionString
                'TR 23/03/2011 -Validate if want to connect to temporal database to do an update)
                If pUpdateConnection Then
                    ConnectionString = GlobalBase.UpdateDatabaseConnectionString
                Else
                    'TR 25/01/2011 -Replace by corresponding value on global base.
                    ConnectionString = GlobalBase.BioSystemsDBConn
                End If

                'validate if connection string is empty to send an error
                If String.IsNullOrEmpty(ConnectionString) Then
                    'Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("Error Reading the Connection String .", "DAOBase", EventLogEntryType.Error, False)
                Else
                    Dim ConnBuilder As New System.Data.SqlClient.SqlConnectionStringBuilder(ConnectionString)
                    DBServerField = ConnBuilder.DataSource
                    If DBServerField.Contains("(local)") Then
                        If System.Net.Dns.GetHostName.Length > 15 Then
                            'TR 30/03/2012 -Cut the DNS name to avoid error, name lenght up to 15.
                            DBServerField = DBServerField.Replace("(local)", System.Net.Dns.GetHostName.Substring(0, 15))
                        Else
                            DBServerField = DBServerField.Replace("(local)", System.Net.Dns.GetHostName)
                        End If
                    End If
                    CurrentDBField = ConnBuilder.InitialCatalog
                    DBLoginField = ConnBuilder.UserID
                    DBPasswordField = ConnBuilder.Password
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DAOBase.GetConnectionString", EventLogEntryType.Error, False)

            End Try

            Return ConnectionString
        End Function

        ''' <summary>
        ''' Begin a Database Transaction over an Open Database Connection
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by RH 23/05/2011 Introduce the Using statement
        ''' </remarks>
        Public Sub BeginTransaction(ByVal pDBConnection As SqlClient.SqlConnection)

            'Dim myLogAcciones As New ApplicationLogManager()

            Try


                GlobalBase.CreateLogActivity(String.Format("{0}.{1}", New System.Diagnostics.StackTrace(2, False).GetFrame(0).GetMethod.ReflectedType.Name, New System.Diagnostics.StackTrace(2, False).GetFrame(0).GetMethod.Name), System.Reflection.MethodInfo.GetCurrentMethod.Name, EventLogEntryType.Information, False)

                Using dbCmd As New SqlClient.SqlCommand()
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED "
                    dbCmd.ExecuteNonQuery()

                    dbCmd.CommandText = " BEGIN TRANSACTION "
                    dbCmd.ExecuteNonQuery()
                End Using

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex) '.Message, "DAOBase.BeginTransaction", EventLogEntryType.Error, False)
                'GlobalBase.CreateLogActivity(ex.Message, "DAOBase.BeginTransaction", EventLogEntryType.Error, False)
            End Try
        End Sub



        ''' <summary>
        ''' Commit the Database Transaction that is opened over the specified Database Connection
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by RH 23/05/2011 Introduce the Using statement
        ''' </remarks>
        Public Sub CommitTransaction(ByVal pDBConnection As SqlClient.SqlConnection)

            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                Dim cmdText As String = " COMMIT TRANSACTION "
                'Dim dbCmd As New SqlClient.SqlCommand
                'dbCmd.Connection = pDBConnection
                'dbCmd.CommandText = cmdText
                'dbCmd.ExecuteNonQuery()

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    dbCmd.ExecuteNonQuery()
                End Using

                GlobalBase.CreateLogActivity(String.Format("{0}.{1}", New System.Diagnostics.StackTrace(1, False).GetFrame(0).GetMethod.ReflectedType.Name, New System.Diagnostics.StackTrace(1, False).GetFrame(0).GetMethod.Name), System.Reflection.MethodInfo.GetCurrentMethod.Name, EventLogEntryType.Information, False)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DAOBase.CommitTransaction", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Rollback the Database Transaction that is opened over the specified Database Connection
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by RH 23/05/2011 Introduce the Using statement
        ''' </remarks>
        Public Sub RollbackTransaction(ByVal pDBConnection As SqlClient.SqlConnection)
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                Dim cmdText As String = " ROLLBACK TRANSACTION "

                'Dim dbCmd As New SqlClient.SqlCommand
                'dbCmd.Connection = pDBConnection
                'dbCmd.CommandText = cmdText
                'dbCmd.ExecuteNonQuery()

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    dbCmd.ExecuteNonQuery()
                End Using

                GlobalBase.CreateLogActivity("", System.Reflection.MethodInfo.GetCurrentMethod.Name, EventLogEntryType.Information, False)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DAOBase.RollbackTransaction", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Verify if the informed Database Connection is open; if not, open a new one
        ''' </summary>
        ''' <param name="pDBConnection">Database Connection</param>
        ''' <returns>GlobalDataTO with the opened Database Connection</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by RH 23/05/2011 Remove unneeded SqlConnection object creation,
        '''             so now there is less presure over the Garbage Collector.
        '''             MI: Removed ByRef as it was not required by function logic, and it's less efficient on .NET
        ''' </remarks>
        ''' 
        Public Function GetOpenDBConnection(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Return GetSafeOpenDBConnection(pDBConnection).GetCompatibleGlobalDataTo
            'Dim openConnection As New GlobalDataTO
            ''Dim dbConnection As New SqlClient.SqlConnection
            'Dim dbConnection As SqlClient.SqlConnection = Nothing

            'Try
            '    If (pDBConnection Is Nothing) Then
            '        'A local Database Connection is opened
            '        dbConnection = New SqlClient.SqlConnection
            '        dbConnection.ConnectionString = GetConnectionString()
            '        dbConnection.Open()
            '        Console.Out.WriteLine("SQL connection created!")
            '    Else
            '        'The opened Database Connection is used
            '        dbConnection = pDBConnection
            '        Console.Out.WriteLine("SQL connection reused!")
            '    End If

            '    openConnection.HasError = False
            '    openConnection.SetDatos = dbConnection

            'Catch ex As Exception
            '    openConnection.HasError = True
            '    openConnection.ErrorCode = "DB_CONNECTION_ERROR"
            '    openConnection.ErrorMessage = ex.Message

            '    'Dim myLogAcciones As New ApplicationLogManager()
            '    GlobalBase.CreateLogActivity(ex.Message, "DAOBase.GetOpenDBConnection", EventLogEntryType.Error, False)

            'End Try

            'Return openConnection
        End Function


        'Future implementation that returns typed DataTo:
        Public Function GetSafeOpenDBConnection(ByRef pDBConnection As SqlClient.SqlConnection) As TypedGlobalDataTo(Of SqlClient.SqlConnection)

            Dim openConnection As New TypedGlobalDataTo(Of SqlClient.SqlConnection)
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                If (pDBConnection Is Nothing) Then
                    'A local Database Connection is opened
                    dbConnection = New SqlClient.SqlConnection
                    dbConnection.ConnectionString = GetConnectionString()
                    dbConnection.Open()
                Else
                    'The opened Database Connection is used
                    dbConnection = pDBConnection
                End If

                openConnection.HasError = False
                openConnection.SetDatos = dbConnection

            Catch ex As Exception
                openConnection.HasError = True
                openConnection.ErrorCode = "DB_CONNECTION_ERROR"
                openConnection.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex)
            End Try

            Return openConnection

        End Function


        ''' <summary>
        ''' Verify if the informed Database Connection is open; if not, open a new one and
        ''' begin a Transaction over it
        ''' </summary>
        ''' <param name="pDBConnection">Database Connection</param>
        ''' <returns>GlobalDataTO with the opened Database Connection</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by RH 23/05/2011 Remove unneeded SqlConnection object creation,
        '''             so now there is less presure over the Garbage Collector.
        ''' </remarks>
        Public Function GetOpenDBTransaction(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim openTransaction As New GlobalDataTO
            'Dim dbConnection As New SqlClient.SqlConnection
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                If (pDBConnection Is Nothing) Then
                    'No Opened Connection has been informed, a local connection is opened
                    dbConnection = New SqlClient.SqlConnection
                    dbConnection.ConnectionString = GetConnectionString()
                    dbConnection.Open()

                    'Begin a DB Transaction over the opened Connection
                    BeginTransaction(dbConnection)
                Else
                    'The opened Connection is used
                    dbConnection = pDBConnection
                End If

                openTransaction.HasError = False
                openTransaction.SetDatos = dbConnection

            Catch ex As Exception
                openTransaction.HasError = True
                openTransaction.ErrorCode = "DB_CONNECTION_ERROR"
                openTransaction.ErrorMessage = ex.Message

                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then
                    dbConnection.Close()
                End If

                GlobalBase.CreateLogActivity(ex.Message, "DAOBase.GetOpenDBTransaction", EventLogEntryType.Error, False)
            End Try
            Return openTransaction
        End Function

        ''' <summary>
        ''' Replace a string in a string value by another string
        ''' </summary>
        ''' <remarks>
        ''' Created by : DL 12/03/2010
        ''' Modified by: SA 02/06/2011 - Added an optional parameter to allow use the function also for DOUBLE values
        ''' </remarks>
        Public Function ReplaceNumericString(ByVal pValue As Single) As String
            Dim resultData As String = ""
            Try
                resultData = pValue.ToString("G20").Replace(",", ".")
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DAOBase.ReplaceNumericString", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Replace a string in a string value by another string
        ''' </summary>
        ''' <remarks>
        ''' Created by : DL 12/03/2010
        ''' Modified by: SA 02/06/2011 - Added an optional parameter to allow use the function also for DOUBLE values
        ''' </remarks>
        Public Function ReplaceNumericString(ByVal pValue As Double) As String
            Dim resultData As String = ""
            Try
                resultData = pValue.ToString("G20").Replace(",", ".")
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DAOBase.ReplaceNumericString", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

    End Module

End Namespace
