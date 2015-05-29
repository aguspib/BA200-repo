Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types

Public Interface IProgrammingController
    Property CalibratorIdFocused As Integer
    Property CalibratorCurveByAnalizer As Boolean

    Function CreateNewCalibrator(ByVal calibratorName As String, ByVal lotNumber As String, ByVal expDatePickUp As Date, ByVal calibNumber As Integer, ByVal userName As String) As CalibratorsDS
    Function UpdatedCalibrator(ByVal calibratorName As String, ByVal lotNumber As String, ByVal expDatePickUp As Date, ByVal calibNumber As Integer, ByVal userName As String) As CalibratorsDS
    Function SaveCalibratorChanges(ByVal pCalibratorsDS As CalibratorsDS, ByVal pCalAction As CalibratorAction, _
                                           ByVal pDeleteCalibratorResult As Boolean, ByVal pDeleteTestCalibratorValue As Boolean, _
                                           ByVal myDeleteCalibratorList As List(Of DeletedCalibratorTO), ByVal isTestParammeterWindows As Boolean) As Boolean
    Function ValidateErrorConcValuesDescOrderGrid(ByRef dataSource As CalibratorsDS, ByRef messageError As String) As Integer
    Function ValidateInputFieldForConcentrations(ByVal dataSource As CalibratorsDS, ByVal rowIndex As Integer) As String

    Sub CalculateFactorFromConcentration(ByRef dataSource As CalibratorsDS)

End Interface
