Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparVirtualRotorsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get the list of Virtual Rotors of an specified type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Type of Virtual Rotors to get</param>
        ''' <param name="pInternalRotor">Optional parameter to indicate when the Virtual Rotor to get
        '''                              is the Internal one to save Reagents Rotor positions</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorsDS with the list of all
        '''          Virtual Rotors of the specified type</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: SA 10/03/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''              SA 18/06/2010 - If the RotorType is not informed, then get all VirtualRotors
        '''              SA 18/04/2011 - Added new optional parameter pInternalRotor and filter the query 
        '''                              according value of this new field
        '''              SA 06/02/2012 - Changed the function template
        ''' </remarks>        
        Public Function ReadByRotorType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, _
                                        Optional ByVal pInternalRotor As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RotorType, VirtualRotorID, VirtualRotorName " & vbCrLf & _
                                                " FROM   tparVirtualRotors " & vbCrLf & _
                                                " WHERE  InternalRotor = " & Convert.ToInt32(IIf(pInternalRotor, 1, 0)) & vbCrLf

                        If (pRotorType.Trim <> "") Then
                            cmdText &= " AND RotorType = '" & pRotorType.Trim & "' " & vbCrLf
                        End If
                        cmdText &= " ORDER BY RotorType, VirtualRotorName "

                        Dim myVirtualRotorsData As New VirtualRotorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVirtualRotorsData.tparVirtualRotors)
                            End Using
                        End Using

                        resultData.SetDatos = myVirtualRotorsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparVirtualRotorsDAO.ReadByRotorType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if a Virtual Rotor exists, searching it by Rotor Type and Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRotorName">Rotor Name</param>
        ''' <returns>GlobalDataTO containing an integer value with the ID of the Virtual Rotor
        '''          when it exists</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: VR 22/12/2009 - Tested: OK
        '''              SA 10/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 26/10/2010 - Added the N preffix for multilanguage when comparing by VirtualRotorName
        '''              SA 19/04/2011 - Added condition to the query; verification should be done excluding Rotors marked as Internal
        '''              
        ''' </remarks>
        Public Function ReadByRotorTypeAndVirtualRotorName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, _
                                                           ByVal pRotorName As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT VirtualRotorID " & vbCrLf & _
                                  " FROM   tparVirtualRotors " & vbCrLf & _
                                  " WHERE  RotorType = '" & pRotorType.Trim & "' " & vbCrLf & _
                                  " AND    UPPER(VirtualRotorName) = UPPER(N'" & pRotorName.Trim.Replace("'", "''") & "') " & vbCrLf & _
                                  " AND    InternalRotor = 0 "
                        '" AND    UPPER(VirtualRotorName) = N'" & pRotorName.Trim.Replace("'", "''").ToUpper & "' " & vbCrLf & _

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        resultData.SetDatos = dbCmd.ExecuteScalar()
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparVirtualRotorsDAO.ReadByRotorTypeAndVirtualRotorName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pVirtualRotorName">Name of the Virtual Rotor to create</param>
        ''' <param name="pInternalRotor">Optional parameter to indicate when the Virtual Rotor to create
        '''                              is the Internal one to save Reagents Rotor positions</param>
        ''' <returns>GlobalDataTO containing an Integer value with the Identifier of the created Virtual Rotor</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: VR 22/12/2009 - Tested: OK
        '''              SA 10/03/2010 - Changed the way of using the DB Connection to fulfill the new template
        '''              SA 26/10/2010 - Added the N preffix for multilanguage for VirtualRotorName and TS_User fields
        '''              SA 18/04/2011 - Added new optional parameter to inform value of new field Internal Rotor
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, _
                               ByVal pVirtualRotorName As String, Optional ByVal pInternalRotor As Boolean = False) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbDataReader As SqlClient.SqlDataReader
            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim objGlobal As New GlobalBase
                    cmdText = " INSERT INTO tparVirtualRotors(RotorType, VirtualRotorName, InternalRotor, TS_User, TS_DateTime) " & _
                              " VALUES ('" & pRotorType & "', " & _
                                     " N'" & pVirtualRotorName.Replace("'", "''") & "', " & _
                                             Convert.ToInt32(IIf(pInternalRotor, 1, 0)) & ", " & _
                                     " N'" & GlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "', " & _
                                      " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                    If (dataToReturn.AffectedRecords = 1) Then
                        cmdText = " SELECT SCOPE_IDENTITY() AS VirtualRotorID "
                        dbCmd.CommandText = cmdText

                        'Execute the SQL sentence 
                        dbDataReader = dbCmd.ExecuteReader()
                        If (dbDataReader.HasRows) Then
                            dbDataReader.Read()
                            dataToReturn.SetDatos = Convert.ToInt32(dbDataReader.Item("VirtualRotorID"))
                        End If
                        dbDataReader.Close()
                        dataToReturn.HasError = False
                    Else
                        dataToReturn.HasError = True
                        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparVirtualRotorsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified Virtual Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pVirtualRotorID">Virtual Rotor Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: SA 10/03/2010 - Changed the way of using the DB Connection to fulfill the new template
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM tparVirtualRotors " & _
                              " WHERE  VirtualRotorID = " & pVirtualRotorID

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                    If (dataToReturn.AffectedRecords = 1) Then
                        dataToReturn.HasError = False
                    Else
                        dataToReturn.HasError = True
                        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparVirtualRotorsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region
    End Class
End Namespace