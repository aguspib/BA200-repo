Imports System.Data.SqlClient
Imports Biosystems.Ax00.Core.Entities.Worksession.Sorting
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.Worksession.Contaminations
    Public Interface IContaminationManager
        Property currentContaminationNumber As Integer
        Property bestContaminationNumber As Integer
        Property bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)

        ''' <summary>
        ''' Method that apply the optimization algorithm defined on the system, in order to avoid as much number of contaminations as possible
        ''' </summary>
        ''' <param name="pCon">SQL connection if available</param>
        ''' <param name="orderTests">OrderTests to be sorted</param>
        ''' <remarks></remarks>
        Sub ApplyOptimizations(pCon As SqlConnection, ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
    End Interface
End Namespace