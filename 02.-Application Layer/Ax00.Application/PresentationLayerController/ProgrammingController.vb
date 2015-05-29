Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types

Public Class ProgrammingController
    Implements IProgrammingController


    Private _calibratorIdFocused As Integer = 0
    Private _calibratorCurveByAnalizer As Boolean


    Public Property CalibratorIdFocused As Integer Implements IProgrammingController.CalibratorIdFocused
        Get
            Return _calibratorIdFocused
        End Get
        Set(value As Integer)
            _calibratorIdFocused = value
        End Set
    End Property

    Public Property CalibratorCurveByAnalizer As Boolean Implements IProgrammingController.CalibratorCurveByAnalizer
        Get
            Return _calibratorCurveByAnalizer
        End Get
        Set(value As Boolean)
            _calibratorCurveByAnalizer = value
        End Set
    End Property


    ''' <summary>
    ''' Load in a CalibratorsDS, all data informed for a new Calibrator to create 
    ''' </summary>
    ''' <returns>Typed DataSet CalibratorsDS containing the data of the Calibrator to add</returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Public Function CreateNewCalibrator(ByVal calibratorName As String, ByVal lotNumber As String, ByVal expDatePickUp As Date, ByVal calibNumber As Integer, ByVal userName As String) As CalibratorsDS _
        Implements IProgrammingController.CreateNewCalibrator

        Dim myCalibratorDS As New CalibratorsDS
        Try
            Dim myCalibratorRow As CalibratorsDS.tparCalibratorsRow
            myCalibratorRow = myCalibratorDS.tparCalibrators.NewtparCalibratorsRow

            'Set a "fake" CalibratorID
            myCalibratorRow.CalibratorID = 1000
            _calibratorIdFocused = 1000

            myCalibratorRow.CalibratorName = calibratorName
            myCalibratorRow.LotNumber = lotNumber
            myCalibratorRow.ExpirationDate = expDatePickUp
            myCalibratorRow.NumberOfCalibrators = calibNumber
            myCalibratorRow.SpecialCalib = False
            myCalibratorRow.InUse = False
            myCalibratorRow.TS_User = userName
            myCalibratorRow.TS_DateTime = DateTime.Now.Date
            myCalibratorRow.IsNew = True
            myCalibratorDS.tparCalibrators.AddtparCalibratorsRow(myCalibratorRow)
        Catch ex As Exception
            Throw
        End Try
        Return myCalibratorDS
    End Function

    ''' <summary>
    ''' Load in a CalibratorsDS all data informed for an existing Calibrator to update 
    ''' </summary>
    ''' <returns>Typed DataSet CalibratorsDS containing the data of the Calibrator to update</returns>
    ''' <remarks>
    ''' Created by: TR 14/02/2011
    ''' </remarks>
    Public Function UpdatedCalibrator(ByVal calibratorName As String, ByVal lotNumber As String, ByVal expDatePickUp As Date, ByVal calibNumber As Integer, ByVal userName As String) As CalibratorsDS _
        Implements IProgrammingController.UpdatedCalibrator
        Dim myCalibratorDS As New CalibratorsDS
        Try
            Dim myCalibratorRow As CalibratorsDS.tparCalibratorsRow
            myCalibratorRow = myCalibratorDS.tparCalibrators.NewtparCalibratorsRow

            myCalibratorRow.CalibratorID = CalibratorIdFocused
            myCalibratorRow.CalibratorName = calibratorName
            myCalibratorRow.LotNumber = lotNumber
            myCalibratorRow.ExpirationDate = expDatePickUp
            myCalibratorRow.NumberOfCalibrators = calibNumber
            myCalibratorRow.SpecialCalib = False
            myCalibratorRow.InUse = False
            myCalibratorRow.TS_User = userName
            myCalibratorRow.TS_DateTime = DateTime.Now.Date
            myCalibratorRow.IsNew = False
            myCalibratorDS.tparCalibrators.AddtparCalibratorsRow(myCalibratorRow)
        Catch ex As Exception
            Throw
        End Try
        Return myCalibratorDS
    End Function

    ''' <summary>
    ''' Add a new Calibrator OR Update data of an existing Calibrator OR Delete a group of selected Calibrators
    ''' </summary>
    ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing the Calibrator to add/update or the group of 
    '''                              Calibrators to delete</param>
    ''' <param name="pCalAction">Action to execute: Add/Update/Delete</param>
    ''' <param name="pDeleteCalibratorResult">When TRUE, it indicates all previous results saved for the Calibrator and all its linked 
    '''                                       Tests/SampleTypes have to be deleted</param>
    ''' <param name="pDeleteTestCalibratorValue">When TRUE, it indicates all theoretical concentration values for the Calibrator and all 
    '''                                          its linked Tests/SampleTypes have to be deleted</param>
    ''' <param name="myDeleteCalibratorList">List of an object DeletedCalibratorTO containing all Calibrators selected to be deleted and,
    '''                                      for each one of them, all its linked Tests/SampleTypes</param>
    ''' <returns>TRUE if the saving was successfully executed</returns>
    ''' <remarks> 
    ''' Created by:  TR 16/02/2011
    ''' </remarks>
    Public Function SaveCalibratorChanges(ByVal pCalibratorsDS As CalibratorsDS, ByVal pCalAction As CalibratorAction, _
                                           ByVal pDeleteCalibratorResult As Boolean, ByVal pDeleteTestCalibratorValue As Boolean, _
                                           ByVal myDeleteCalibratorList As List(Of DeletedCalibratorTO), ByVal isTestParammeterWindows As Boolean) As Boolean Implements IProgrammingController.SaveCalibratorChanges
        Dim myResults As Boolean
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalibratorsDelegate As New CalibratorsDelegate

            Select Case pCalAction
                Case CalibratorAction.Create
                    myGlobalDataTO = myCalibratorsDelegate.Save(Nothing, pCalibratorsDS, _
                                                                Nothing, False, pDeleteCalibratorResult, pDeleteTestCalibratorValue, _
                                                                AnalyzerController.Instance.Analyzer.ActiveAnalyzer, AnalyzerController.Instance.Analyzer.ActiveWorkSession)
                    Exit Select
                Case CalibratorAction.Edit
                    myGlobalDataTO = myCalibratorsDelegate.Save(Nothing, pCalibratorsDS, _
                                                                Nothing, False, pDeleteCalibratorResult, pDeleteTestCalibratorValue, _
                                                                AnalyzerController.Instance.Analyzer.ActiveAnalyzer, AnalyzerController.Instance.Analyzer.ActiveWorkSession)
                    Exit Select
                Case CalibratorAction.Delete
                    myGlobalDataTO = myCalibratorsDelegate.Save(Nothing, pCalibratorsDS, _
                                                                myDeleteCalibratorList, isTestParammeterWindows, pDeleteCalibratorResult, _
                                                                pDeleteTestCalibratorValue, AnalyzerController.Instance.Analyzer.ActiveAnalyzer, AnalyzerController.Instance.Analyzer.ActiveWorkSession)
                    Exit Select
            End Select

            myResults = (Not myGlobalDataTO.HasError)
        Catch ex As Exception
            Throw
        End Try
        Return myResults
    End Function

    ''' <summary>
    ''' Validate all Theorical Concentration values have been informed and they are in descendent order
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 18/02/2011
    ''' Modified by: TR 09/10/2011 - Changed all convert to INT by convert to DOUBLE
    '''              WE 26/01/2015 - BA-2047: Solved bug in which the program flow could enter the save process when the
    '''                              Theoretical Concentration entered by the user was rounded to 0 (zero).
    ''' </remarks>
    Public Function ValidateErrorConcValuesDescOrderGrid(ByRef dataSource As CalibratorsDS, ByRef messageError As String) As Integer Implements IProgrammingController.ValidateErrorConcValuesDescOrderGrid
        Dim numRow As Integer = 0
        Dim previousConcentration As Single = 0
        For Each concentrationRow As CalibratorsDS.tparTestCalibratorValuesRow In dataSource.tparTestCalibratorValues



            'Validate the Theorical Concentration is informed for the row and also that it is numeric
            If (Not concentrationRow.IsTheoricalConcentrationNull() AndAlso IsNumeric(concentrationRow.TheoricalConcentration)) Then
                ' WE 26/01/2015 (BA-2047) - added check to prevent entering the save process when the Theoretical Conc. value is rounded to 0 (zero).
                If (concentrationRow.TheoricalConcentration <= 0) OrElse concentrationRow.TheoricalConcentration = 0 Then
                    messageError = GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString()
                    Return numRow
                End If

                ''Compare value with the Theorical Concentration on the previous row
                'If (numRow <> 0 AndAlso previousConcentration <= concentrationRow.TheoricalConcentration) Then
                '    messageError = GlobalEnumerates.Messages.VALIDCONCENTRATIONVALUE.ToString()
                '    Return numRow
                'End If
            Else
                messageError = GlobalEnumerates.Messages.REQUIRED_VALUE.ToString()
                Return numRow
            End If
            numRow += 1
            previousConcentration = concentrationRow.TheoricalConcentration
        Next
        Return -1
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <param name="rowIndex"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Comprobar celda editada 
    ''' - No null
    ''' - No cero
    ''' - No negativo (es posible?)
    ''' - No <> numero (es posible?)
    ''' - No repetido
    ''' </remarks>
    Public Function ValidateInputFieldForConcentrations(ByVal dataSource As CalibratorsDS, ByVal rowIndex As Integer) As String Implements IProgrammingController.ValidateInputFieldForConcentrations
        If dataSource.tparTestCalibratorValues(rowIndex).IsTheoricalConcentrationNull Then Return GlobalEnumerates.Messages.NOT_NULL_VALUE.ToString()

        If dataSource.tparTestCalibratorValues(rowIndex).TheoricalConcentration <= 0 Then Return GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString()

        If (From c In dataSource.tparTestCalibratorValues _
           Where c.TheoricalConcentration = dataSource.tparTestCalibratorValues(rowIndex).TheoricalConcentration _
           Select c).Count > 1 Then Return GlobalEnumerates.Messages.DUPLICATE_CODE.ToString()

        Return String.Empty
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dataSource"></param>
    ''' <remarks></remarks>
    Public Sub CalculateFactorFromConcentration(ByRef dataSource As CalibratorsDS) Implements IProgrammingController.CalculateFactorFromConcentration
        Dim localDTCalibratorValues As CalibratorsDS.tparTestCalibratorValuesDataTable = dataSource.tparTestCalibratorValues

        Dim maxConcentration = (From x In dataSource.tparTestCalibratorValues _
                                Where x.TheoricalConcentration = localDTCalibratorValues.Max(Function(row) row.TheoricalConcentration) _
                                Select x.TheoricalConcentration).FirstOrDefault()

        For Each concentrationRow As CalibratorsDS.tparTestCalibratorValuesRow In dataSource.tparTestCalibratorValues
            If (concentrationRow.TheoricalConcentration = maxConcentration) Then
                concentrationRow.KitConcentrationRelation = 1
            Else
                'concentrationRow.KitConcentrationRelation = CType(Math.Round(concentrationRow.TheoricalConcentration / maxConcentration, 3), Single)
                concentrationRow.KitConcentrationRelation = concentrationRow.TheoricalConcentration / maxConcentration

            End If
        Next
    End Sub
End Class
