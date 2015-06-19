Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public interface IAnalyzerManagerFlagsDelegate
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pFlagID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested pending</remarks>
        Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                             ByVal pFlagID As String) As GlobalDataTO

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested OK</remarks>
        Function ReadByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerFlagsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Testing OK</remarks>
        Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerFlagsDS As AnalyzerManagerFlagsDS) As GlobalDataTO

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerFlagsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Testing pending</remarks>
        Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerFlagsDS As AnalyzerManagerFlagsDS) As GlobalDataTO

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pFlagID" ></param>
        ''' <param name="pNewValue" ></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Testing pending</remarks>
        Function UpdateFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                   ByVal pFlagID As String, ByVal pNewValue As String) As GlobalDataTO

        ''' <summary>
        ''' Reset all Software flags
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pLeaveConnectFlag" ></param>
        ''' <returns></returns>
        ''' <remarks>Modified AG 21/06/2012 - add optional parameter pLeaveConnectFlag</remarks>
        Function ResetFlags(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pLeaveConnectFlag As Boolean = True) As GlobalDataTO

        ''' <summary>
        ''' Read all flags with status = pValue (if pReadWithSameValue = True)
        ''' Read all flags with status != pValue (if pReadWithSameValue = False)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pValue"></param>
        ''' <param name="pReadWithSameValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 09/03/2012</remarks>
        Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pValue As String, _
                                     ByVal pReadWithSameValue As Boolean) As GlobalDataTO

        ''' <summary>
        ''' Delete Flags from AnalyzerID specified
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XB 03/05/2013</remarks>
        Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
    end interface
End NameSpace