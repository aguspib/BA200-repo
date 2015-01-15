Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Partial Public Class tparPreviousTestControlLotsDAO
    Inherits DAOBase

#Region "CRUD Methods"

    ''' <summary>
    ''' Receive a Control Identifier and copy all data of the Tests/SampleTypes linked to the current Lot to the equivalent 
    ''' Previous Saved Lot structure
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  DL 01/04/2011
    ''' Modified by: SA 10/05/2011 
    ''' </remarks>
    Public Function SavePreviousLotTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " INSERT INTO tparPreviousTestControlLots(ControlID, LotNumber, TestID, SampleType, MinConcentration, " & vbCrLf & _
                                                                                " MaxConcentration, TargetMean, TargetSD) " & vbCrLf & _
                                        " SELECT TC.ControlID, C.LotNumber, TC.TestID, TC.SampleType, TC.MinConcentration, " & vbCrLf & _
                                               " TC.MaxConcentration, TC.TargetMean, TC.TargetSD " & vbCrLf & _
                                        " FROM  tparControls C INNER JOIN tparTestControls TC ON C.ControlID = TC.ControlID " & vbCrLf & _
                                        " WHERE C.ControlID = " & pControlID

                'cmdText &= "" & vbCrLf
                'cmdText &= "          ( ControlID" & vbCrLf
                'cmdText &= "          , LotNumber" & vbCrLf
                'cmdText &= "          , TestID" & vbCrLf
                'cmdText &= "          , SampleType" & vbCrLf
                'cmdText &= "          , MinConcentration" & vbCrLf
                'cmdText &= "          , MaxConcentration" & vbCrLf
                'cmdText &= "          , TargetMean" & vbCrLf
                'cmdText &= "          , TargetSD)" & vbCrLf
                'cmdText &= "     SELECT TC.ControlID " & vbCrLf
                'cmdText &= "          , C.LotNumber" & vbCrLf
                'cmdText &= "          , TC.TestID" & vbCrLf
                'cmdText &= "          , TC.SampleType" & vbCrLf
                'cmdText &= "          , TC.MinConcentration" & vbCrLf
                'cmdText &= "          , TC.MaxConcentration" & vbCrLf
                'cmdText &= "          , TC.TargetMean" & vbCrLf
                'cmdText &= "          , TC.TargetSD" & vbCrLf
                'cmdText &= "       FROM tparTestControls TC INNER JOIN tparControls C ON TC.ControlID = C.ControlID" & vbCrLf
                'cmdText &= "      WHERE C.ControlID = " & pControlID

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.SavePreviousLotTests", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Receive a Control Identifier and copy all data of the Tests/SampleTypes/TestType 
    ''' linked to thecurrent Lot, to the equivalent Previous Saved Lot structure
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: RH 15/06/2012 
    ''' </remarks>
    Public Function SavePreviousLotTestsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
            Else
                Dim cmdText As String
                cmdText = String.Format(" INSERT INTO tparPreviousTestControlLots(ControlID, LotNumber, TestID, SampleType, " & vbCrLf & _
                                        "                         MinConcentration, MaxConcentration, TargetMean, TargetSD, TestType) " & vbCrLf & _
                                        " SELECT TC.ControlID, C.LotNumber, TC.TestID, TC.SampleType, TC.MinConcentration, " & vbCrLf & _
                                        "        TC.MaxConcentration, TC.TargetMean, TC.TargetSD, TC.TestType " & vbCrLf & _
                                        " FROM  tparControls C INNER JOIN tparTestControls TC ON C.ControlID = TC.ControlID " & vbCrLf & _
                                        " WHERE C.ControlID = {0} ", pControlID)

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using
            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.SavePreviousLotTests", EventLogEntryType.Error, False)
        End Try

        Return resultData
    End Function

    ''' <summary>
    ''' Get all Tests/Sample Types linked to the previous saved Lot of the informed Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Tests/SampleTypes 
    '''          linked to the informed Control for its previous saved Lot
    ''' </returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 10/05/2011 - Parameter pControlID cannot be optional 
    ''' </remarks>
    Public Function ReadByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String
                    cmdText = " SELECT PTCL.*, T.TestName, T.DecimalsAllowed, T.PreloadedTest, T.TestPosition, T.InUse, " & vbCrLf & _
                                     " TS.RejectionCriteria, MD.FixedItemDesc As MeasureUnit " & vbCrLf & _
                              " FROM   tparPreviousTestControlLots PTCL INNER JOIN tparTests T        ON PTCL.TestID = T.TestID " & vbCrLf & _
                                                                      " INNER JOIN tparTestSamples TS ON PTCL.TestID = TS.TestID AND PTCL.SampleType = TS.SampleType " & vbCrLf & _
                                                                      " INNER JOIN tcfgMasterData MD  ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
                              " WHERE  PTCL.ControlID = " & pControlID & vbCrLf & _
                              " AND    MD.SubTableID  = '" & MasterDataEnum.TEST_UNITS.ToString & "' "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim myTestControlsDS As New TestControlsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(myTestControlsDS.tparTestControls)

                    resultData.SetDatos = myTestControlsDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.ReadByControlID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function


    '''' <summary>
    '''' Get the list of all previouse control lots
    '''' </summary>
    '''' <param name="pDBConnection">Open DB Connection</param>
    '''' <returns>
    '''' GlobalDataTO containing a typed DataSet ControlDS with the list of all defined Controls
    '''' </returns>
    '''' <remarks></remarks>
    'Public Function GetPreviousLotTests(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pControlID As Integer = -1) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO
    '    Dim dbConnection As New SqlClient.SqlConnection

    '    Try
    '        resultData = GetOpenDBConnection(pDBConnection)
    '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
    '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
    '            If (Not dbConnection Is Nothing) Then
    '                Dim cmdText As String = ""

    '                cmdText &= "  SELECT PTCL.ControlID" & vbCrLf
    '                cmdText &= "       , PTCL.LotNumber" & vbCrLf
    '                cmdText &= "       , PTCL.TestID" & vbCrLf
    '                cmdText &= "       , PTCL.SampleType" & vbCrLf
    '                cmdText &= "       , PTCL.MinConcentration" & vbCrLf
    '                cmdText &= "       , PTCL.MaxConcentration" & vbCrLf
    '                cmdText &= "       , PTCL.TargetMean" & vbCrLf
    '                cmdText &= "       , PTCL.TargetSD" & vbCrLf
    '                cmdText &= "       , T.DecimalsAllowed" & vbCrLf
    '                cmdText &= "       , T.InUse" & vbCrLf
    '                cmdText &= "       , T.TestName" & vbCrLf
    '                cmdText &= "       , TS.SampleType" & vbCrLf
    '                cmdText &= "       , TS.RejectionCriteria" & vbCrLf
    '                cmdText &= "       , T.PreloadedTest" & vbCrLf
    '                cmdText &= "    FROM tparPreviousTestControlLots PTCL INNER JOIN tparTests T ON (ptcl.testid = t.testid)" & vbCrLf
    '                cmdText &= "                                          INNER JOIN tparTestSamples TS on (t.testid = ts.testid)" & vbCrLf

    '                If pControlID > 0 Then cmdText &= " WHERE ptcl.ControlID = " & pControlID

    '                Dim dbCmd As New SqlClient.SqlCommand
    '                dbCmd.Connection = dbConnection
    '                dbCmd.CommandText = cmdText

    '                'Fill the DataSet to return 
    '                Dim myTestControlsDS As New TestControlsDS
    '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
    '                dbDataAdapter.Fill(myTestControlsDS.tparTestControls)

    '                resultData.SetDatos = myTestControlsDS
    '                resultData.HasError = False
    '            End If
    '        End If
    '    Catch ex As Exception
    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.GetPreviousLotTests", EventLogEntryType.Error, False)
    '    Finally
    '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
    '    End Try
    '    Return resultData
    'End Function

    ''' <summary>
    ''' Delete the specified Test/SampleType linked to the previous saved Lot of the informed Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Identifier of the Control to delete</param>        
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">SampleType Code</param>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  DL 05/04/2011
    ''' Modified by: SA 10/05/2011 - New implementation, the previous one was wrong
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, ByVal pTestID As Integer, _
                           ByVal pSampleType As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparPreviousTestControlLots " & vbCrLf & _
                                        " WHERE  ControlID  = " & pControlID & vbCrLf & _
                                        " AND    TestID     = " & pTestID & vbCrLf & _
                                        " AND    SampleType = '" & pSampleType & "' "

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.Delete", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all Tests/SampleTypes linked to the previous saved Lot of the informed Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Identifier of the Control to delete</param>        
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  SA 10/05/2011
    ''' </remarks>
    Public Function DeleteByControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparPreviousTestControlLots " & vbCrLf & _
                                        " WHERE  ControlID  = " & pControlID

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.DeleteByControlID", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all information for a previous Control Lots for an specific Test, and optionally, a 
    ''' Sample Type and/or a list of Controls (the list will contain those Controls that have to remain linked
    ''' to the Test/SampleType)
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type; optional parameter</param>
    ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  TR 07/04/2011
    ''' Modified by: SA 11/05/2011 - Added optional parameter pControlIDList
    ''' </remarks>
    Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                   Optional ByVal pControlIDList As String = "") As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparPreviousTestControlLots " & vbCrLf & _
                                        " WHERE  TestID = " & pTestID & vbCrLf

                'Filter data for the specified optional parameters when they are informed
                If (pSampleType <> "") Then cmdText &= " AND SampleType = '" & pSampleType & "'"
                If (pControlIDList <> "") Then cmdText &= " AND ControlID NOT IN (" & pControlIDList & ") "

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, " tparPreviousTestControlLotsDAO.DeleteByTestID ", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete the specified Test/SampleType linked to the previous saved Lot of the informed Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Identifier of the Control to delete</param>        
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">SampleType Code</param>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  DL 05/04/2011
    ''' Modified by: SA 10/05/2011 - New implementation, the previous one was wrong
    ''' Modified by: RH 11/06/2012 - New parameter TestType.
    ''' </remarks>
    Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, ByVal pTestID As Integer, _
                           ByVal pSampleType As String, ByVal pTestType As String) As GlobalDataTO

        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparPreviousTestControlLots " & vbCrLf & _
                                        " WHERE  ControlID  = " & pControlID & vbCrLf & _
                                        " AND    TestID     = " & pTestID & vbCrLf & _
                                        " AND    SampleType = '" & pSampleType & "' " & vbCrLf & _
                                        " AND    TestType = '" & pTestType & "' "

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using

            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.Delete", EventLogEntryType.Error, False)

        End Try

        Return resultData
    End Function

    ''' <summary>
    ''' Delete all information for a previous Control Lots for an specific Test, and optionally, a 
    ''' Sample Type and/or a list of Controls (the list will contain those Controls that have to remain linked
    ''' to the Test/SampleType)
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type; optional parameter</param>
    ''' <param name="pControlIDList">List of ControlIDs separated by (,). Optional parameter</param>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  TR 07/04/2011
    ''' Modified by: SA 11/05/2011 - Added optional parameter pControlIDList
    ''' Modified by: RH 11/06/2012 - New parameter TestType.
    ''' </remarks>
    Public Function DeleteByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                      ByVal pTestType As String, _
                                      Optional ByVal pSampleType As String = "", _
                                      Optional ByVal pControlIDList As String = "") As GlobalDataTO

        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparPreviousTestControlLots " & vbCrLf & _
                                        " WHERE  TestID = " & pTestID & vbCrLf & _
                                        " AND    TestType = '" & pTestType & "' " & vbCrLf

                'Filter data for the specified optional parameters when they are informed
                If Not String.IsNullOrEmpty(pSampleType) Then cmdText &= " AND SampleType = '" & pSampleType & "'"

                If Not String.IsNullOrEmpty(pControlIDList) Then cmdText &= " AND ControlID NOT IN (" & pControlIDList & ") "

                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using

            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, " tparPreviousTestControlLotsDAO.DeleteByTestID ", EventLogEntryType.Error, False)
        End Try

        Return resultData
    End Function

    '''' <summary>
    '''' Create a new previous control lot
    '''' </summary>
    '''' <param name="pDBConnection">Open DB Connection</param>
    '''' <param name="pTestControlPreviousLotRow">Typed DataSet ControlDS with data of the Control to add</param>
    '''' <returns>GlobalDataTO containing a typed DataSet ControlDS with all data of the added control or error information
    '''' </returns>
    '''' <remarks>
    '''' Created by:  DL 1/04/2011
    '''' </remarks>
    'Public Function SavePreviousLot(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestControlPreviousLotRow As TestControlsDS.tparTestControlsRow) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO

    '    Try
    '        If (pDBConnection Is Nothing) Then
    '            resultData.HasError = True
    '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
    '        Else
    '            Dim cmdText As String = ""

    '            cmdText &= "INSERT INTO tparPreviousTestControlLots" & vbCrLf
    '            cmdText &= "          ( ControlID" & vbCrLf
    '            cmdText &= "          , LotNumber" & vbCrLf
    '            cmdText &= "          , TestID" & vbCrLf
    '            cmdText &= "          , SampleType" & vbCrLf
    '            cmdText &= "          , MinConcentration" & vbCrLf
    '            cmdText &= "          , MaxConcentration" & vbCrLf
    '            cmdText &= "          , TargetMean" & vbCrLf
    '            cmdText &= "          , TargetSD)" & vbCrLf
    '            cmdText &= "   VALUES " & vbCrLf
    '            cmdText &= "          ( " & pTestControlPreviousLotRow.ControlID & vbCrLf
    '            cmdText &= "          , '" & pTestControlPreviousLotRow.LotNumber & "'" & vbCrLf
    '            cmdText &= "          , " & pTestControlPreviousLotRow.TestID & vbCrLf
    '            cmdText &= "          , '" & pTestControlPreviousLotRow.SampleType & "'" & vbCrLf

    '            If (pTestControlPreviousLotRow.IsMinConcentrationNull) Then
    '                cmdText &= "          , NULL" & vbCrLf
    '            Else
    '                cmdText &= "          , " & ReplaceNumericString(pTestControlPreviousLotRow.MinConcentration) & vbCrLf
    '            End If

    '            If (pTestControlPreviousLotRow.IsMaxConcentrationNull) Then
    '                cmdText &= "          , NULL" & vbCrLf
    '            Else
    '                cmdText &= "          , " & ReplaceNumericString(pTestControlPreviousLotRow.MaxConcentration) & vbCrLf
    '            End If

    '            If (pTestControlPreviousLotRow.IsTargetMeanNull) Then
    '                cmdText &= "          , NULL" & vbCrLf
    '            Else
    '                cmdText &= "          , " & ReplaceNumericString(pTestControlPreviousLotRow.TargetMean) & vbCrLf
    '            End If

    '            If (pTestControlPreviousLotRow.IsTargetSDNull) Then
    '                cmdText &= "          , NULL" & vbCrLf
    '            Else
    '                cmdText &= "          , " & ReplaceNumericString(pTestControlPreviousLotRow.TargetSD) & vbCrLf
    '            End If

    '            cmdText &= "          )"

    '            'cmdText &= "     SELECT C.ControlID " & vbCrLf
    '            'cmdText &= "          , C.LotNumber" & vbCrLf
    '            'cmdText &= "          , TC.TestID" & vbCrLf
    '            'cmdText &= "          , C.SampleType" & vbCrLf
    '            'cmdText &= "          , TC.MinConcentration" & vbCrLf
    '            'cmdText &= "          , TC.MaxConcentration" & vbCrLf
    '            'cmdText &= "          , TC.TargetMean" & vbCrLf
    '            'cmdText &= "          , TC.TargetSD" & vbCrLf
    '            'cmdText &= "       FROM tparControls C INNER JOIN tparTestControls TC on (C.ControlID = TC.ControlID)" & vbCrLf
    '            'cmdText &= "      WHERE C.ControlID = " & pControlID

    '            Dim dbCmd As New SqlClient.SqlCommand
    '            dbCmd.Connection = pDBConnection
    '            dbCmd.CommandText = cmdText

    '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
    '            If (resultData.AffectedRecords > 0) Then
    '                resultData.HasError = False
    '            Else
    '                resultData.HasError = True
    '                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '            End If
    '        End If

    '    Catch ex As Exception
    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "tparPreviousTestControlLotsDAO.SavePreviousLot", EventLogEntryType.Error, False)
    '    End Try
    '    Return resultData
    'End Function

#End Region

End Class
