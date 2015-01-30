''' MIC
''' <summary>
''' This is an OO interface that is implemented in all DataSet rows that attend the graphical curve functionality
''' </summary>
''' <remarks>Currently used by the results monitor curve data to retrieve information for several, different typed datasets</remarks>
Public Interface IMonitorCurveResultsRow
    ' ReSharper disable once InconsistentNaming
    Property OrderTestID As Integer
    Property MultiItemNumber As Integer
    Property RerunNumber As Integer
End Interface
