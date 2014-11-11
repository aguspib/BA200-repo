Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Entities

    Public MustInherit Class BaseLineEntity
        Implements IBaseLineEntity

#Region "Abstract methods"
        ''' <summary>
        ''' Get the last ADJUST base lines
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 04/11/2014 BA-2065 (refactoring method GetLatestBaseLines in this class)
        ''' </remarks>
        Public MustOverride Function GetCurrentAdjustBaseLineValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

#End Region

#Region "Events definition"
        'Inform the well status changes due base line results
        Public Event WellReactionsChanges(ByVal pReactionsRotorDS As ReactionsRotorDS) Implements IBaseLineEntity.WellReactionsChanges

#End Region

#Region "Class structures"

        'Last adjust base line counts, ...
        Private Structure AdjustBaseLine
            Dim mainLight As List(Of Integer) 'Main light (photodiode position) = value <integer>
            Dim refLight As List(Of Integer) 'Ref light (photodiode position) = value <integer>
            Dim mainDark As List(Of Integer) 'Main dark (photodiode position) = value <integer>
            Dim refDark As List(Of Integer) 'Ref dark (photodiode position) = value <integer>
            Dim enabled As List(Of Boolean) 'Enabled (Photodiode position) = value <boolean> 'indicates if the led is enabled or not
            Dim DAC As List(Of Single) 'DAC (photodiode position) = value <single>
            Dim IT As List(Of Single) 'Integration Time (photodiode position) = value <single>
            Dim rejected As Boolean
        End Structure

        'Last well base line counts, ...
        Private Structure wellBaseLine
            Dim mainLight As List(Of Integer) 'Main light (photodiode position) = value <integer>
            Dim refLight As List(Of Integer) 'Ref light (photodiode position) = value <integer>
            Dim Abs As List(Of Single) 'Absorbance (photodiode position) = value <single>
            Dim well As Integer
            Dim rejected As Boolean
        End Structure

        Private Structure wellAbsorbances
            Dim Abs As List(Of Single) 'Absorbance list for the wells used in initialization well base line structure index (1..10)
        End Structure

        'Initialization well rejection parameters
        Private Structure initializationParameters
            Dim absAVG As List(Of Single) 'avg ABS (photodiode position) = value <single>
            Dim absSD As List(Of Single) 'Standard deviation ABS (photodiode position) = value <single>
            Dim absByWELL() As wellAbsorbances 'well ABS (photodiode position) = List of absorbances (1..10)

            Dim wellUsed As List(Of Integer) 'Well used in the well base line control well (1 .. 10) = value <integer>
            Dim rejected As List(Of Boolean) 'Well is rejected in the well base line control well (1 .. 10) = value <boolean>

            Dim alarm As GlobalEnumerates.Alarms
            Dim initializationParameterItem As Integer 'Initialitzation parameter iteration number
            Dim initRejected As Boolean 'Initialization rejected
            Dim wellsNotUsedAfterALight As Integer 'After ALIGHT the first BL well can not be used due are not filled
        End Structure

#End Region

#Region "Declarations and Attributes"

        'Field limits and Sw parameters variables (default value based on SPEC document but they are initialized in method GetClassParameterValues)
        'Adjust base line limits
        Private DAC_LIMIT_MIN As Single = (1 / 1.4)
        Private DAC_LIMIT_MAX As Single = 2
        Private MAINLIGHT_COUNTS_LIMIT_MIN As Integer = 600000
        Private MAINLIGHT_COUNTS_LIMIT_MAX As Integer = 930000
        Private REFLIGHT_COUNTS_LIMIT_MIN As Integer = 100000
        Private REFLIGHT_COUNTS_LIMIT_MAX As Integer = 930000
        Private MAINDARK_COUNTS_LIMIT_MIN As Integer = 3000
        Private MAINDARK_COUNTS_LIMIT_MAX As Integer = 6000
        Private REFDARK_COUNTS_LIMIT_MIN As Integer = 3000
        Private REFDARK_COUNTS_LIMIT_MAX As Integer = 6000
        Private BL_WELLREJECT_INI_SD As Single = 0.027
        Private BL_WELLREJECT_INI_WELLNUMBER As Integer = 10

        'Well base line initialization process limits
        Private BL_WELLREJECT_INI_RANGE_LIMIT_MIN As Single = -0.08
        Private BL_WELLREJECT_INI_RANGE_LIMIT_MAX As Single = 0.08
        Private BL_WELLREJECT_INI_ABS_LIMIT_MIN As Single = -0.08
        Private BL_WELLREJECT_INI_ABS_LIMIT_MAX As Single = 0.08
        Private BL_WELLREJECTED_INI_LIMIT_MIN As Single = 1
        Private BL_WELLREJECTED_INI_LIMIT_MAX As Single = 3
        Private BL_WELLREJECT_ITEMS_NOTUSED As Integer = 7

        'Well base line (running) process limits
        Private BL_WELLREJECT_RANGE_LIMIT_MIN As Single = -0.08
        Private BL_WELLREJECT_RANGE_LIMIT_MAX As Single = 0.08
        Private BL_WELLREJECT_ABS_LIMIT_MIN As Single = -0.08
        Private BL_WELLREJECT_ABS_LIMIT_MAX As Single = 0.08
        Private BL_WELLREJECT_SD As Single = 0.027

        Private PATH_LENGHT As Single = 6.11
        Private LIMIT_ABS As Single = 3.3
        Private MAX_REJECTED_WELLS As Integer = 20 'Max rejected wells allowed in one reactions rotor
        Private MAX_REACTROTOR_WELLS As Integer = 120 'Max wells in reactions rotor

        Private WASHSTATION_BLW_LIMIT_MIN As Integer = -7 'BLW value - 7 ... BLW value: wells in washing station
        Private WASHSTATION_BLW_LIMIT_MAX As Integer = 3 'BLW value ... BLW value + 3 : wells in washing station

        Private myAnalyzerID As String = ""

        Private fieldLimitsAttribute As New FieldLimitsDS
        Private swParametersAttribute As New ParametersDS
        Private instrumentAdjustments As New SRVAdjustmentsDS
        Private validAlightAttribute As Boolean = False
        Private existsAlightResultsAttribute As Boolean = False 'AG 20/06/2012

        Private adjustBL As New AdjustBaseLine
        Private wellBL As New wellBaseLine
        Private rejectionParameters As New initializationParameters
        Private InitializationComplete As Boolean = False
        Private myListOfRejectedWells As New List(Of Integer)

        Private BL_CONSECUTIVEREJECTED_WELL As Integer = 30 'AG 04/06/2012 - Max consecutive rejected wells allowed
        Private consecutiveRejectedWells As Integer = 0 'AG 04/06/2012 - Number of consecutive wells rejected
        Private exitRunningTypeAttribute As Integer = 0 'AG 04/06/2012 - 0 do not leave running mode
        '                                                1 - Number of consecutive rejected wells > Limit while update FIFO phase (Sw will leave running sending END instruction)
        '                                                2 - Number of consecutive rejected wells > Limit while init FIFO phase (Sw will leave running sending STANDBY instruction)
        '                                                    RPM 06/09/2012 NO!!! use END instruction also in case 2

        Private BaseLineTypeForWellRejectAttribute As String = GlobalEnumerates.BaseLineType.STATIC.ToString 'AG 11/11/2014 BA-2065
#End Region

#Region "Properties"

        Public WriteOnly Property fieldLimits() As FieldLimitsDS Implements IBaseLineEntity.fieldLimits
            Set(ByVal value As FieldLimitsDS)
                fieldLimitsAttribute = value
                GetClassParameterValues(True)
            End Set
        End Property

        Public WriteOnly Property SwParameters() As ParametersDS Implements IBaseLineEntity.SwParameters
            Set(ByVal value As ParametersDS)
                swParametersAttribute = value
                GetClassParameterValues(False)
            End Set
        End Property

        Public WriteOnly Property Adjustments() As SRVAdjustmentsDS Implements IBaseLineEntity.Adjustments
            Set(ByVal value As SRVAdjustmentsDS)
                instrumentAdjustments = value
            End Set
        End Property

        Public Property validALight() As Boolean Implements IBaseLineEntity.validALight
            Get
                Return validAlightAttribute
            End Get
            Set(ByVal value As Boolean)
                validAlightAttribute = value
            End Set
        End Property

        Public ReadOnly Property exitRunningType() As Integer Implements IBaseLineEntity.exitRunningType 'AG 04/06/2012 
            Get
                Return exitRunningTypeAttribute
            End Get
        End Property

        Public ReadOnly Property existsAlightResults() As Boolean Implements IBaseLineEntity.existsAlightResults
            Get
                Return existsAlightResultsAttribute
            End Get

        End Property

        'AG 11/11/2014 BA-2065
        Public Property BaseLineTypeForWellReject() As BaseLineType Implements IBaseLineEntity.BaseLineTypeForWellReject
            Set(ByVal value As BaseLineType)
                BaseLineTypeForWellRejectAttribute = value
            End Set
            Get
                Return BaseLineTypeForWellRejectAttribute
            End Get
        End Property
#End Region

#Region "Constructor and other initialization methods"


        ''' <summary>
        ''' During class initialization gets the latest adjust base line values and latest well base line used in initialization parameter
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo with setDatos as GlobalEnumerates.Alarms</returns>
        ''' <remarks>AG 04/05/2011</remarks>
        Public Function GetLatestBaseLines(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                            ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String) As GlobalDataTO Implements IBaseLineEntity.GetLatestBaseLines
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        myAnalyzerID = pAnalyzerID

                        consecutiveRejectedWells = 0 'Initialize counter
                        exitRunningTypeAttribute = 0

                        InitStructures(True, True)

                        'Get latest base line with adjust 
                        Dim myAlarm As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE

                        'AG 04/11/2014 BA-2065 refactoring
                        'Dim blinesDelg As New WSBLinesDelegate
                        'resultData = blinesDelg.GetCurrentBaseLineValues(dbConnection, pAnalyzerID)
                        resultData = GetCurrentAdjustBaseLineValues(dbConnection, pAnalyzerID)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim ALineDS As New BaseLinesDS
                            ALineDS = CType(resultData.SetDatos, BaseLinesDS)
                            If ALineDS.twksWSBaseLines.Rows.Count > 0 Then
                                validAlightAttribute = False
                                resultData = ControlAdjustBaseLine(dbConnection, ALineDS)

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myAlarm = CType(resultData.SetDatos, GlobalEnumerates.Alarms)

                                    'If adjust base line is OK then get current wells in rejection parameters
                                    If myAlarm = GlobalEnumerates.Alarms.NONE Then
                                        Dim blinesWellDelegate As New WSBLinesByWellDelegate
                                        resultData = blinesWellDelegate.GetMeanWellBaseLineValues(dbConnection, pAnalyzerID, pWorkSessionID)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            Dim wellLineDS As New BaseLinesDS
                                            wellLineDS = CType(resultData.SetDatos, BaseLinesDS)
                                            If wellLineDS.twksWSBaseLines.Rows.Count > 0 Then
                                                resultData = ControlWellBaseLine(dbConnection, True, wellLineDS)
                                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                    myAlarm = CType(resultData.SetDatos, GlobalEnumerates.Alarms)
                                                End If
                                            End If
                                        End If
                                    End If

                                    'AG 07/03/2012 - ControlWellBaseLine only evaluates the results. If no error we must also
                                    'evaluate the rejected wells number, the days from the last rotor change,... and other conditions
                                    If myAlarm = GlobalEnumerates.Alarms.NONE Then
                                        Dim analyzerReactRotor As New AnalyzerReactionsRotorDelegate
                                        resultData = analyzerReactRotor.ChangeReactionsRotorRecommended(dbConnection, pAnalyzerID, pAnalyzerModel)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            If String.Compare(CType(resultData.SetDatos, String), "", False) <> 0 Then
                                                myAlarm = GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                                            End If
                                        End If
                                    End If
                                    'AG 07/03/2012

                                End If

                            Else
                                existsAlightResultsAttribute = False 'AG 20/06/2012
                            End If

                        Else
                            InitStructures(True, True)
                        End If
                        resultData.SetDatos = myAlarm
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.GetLatestBaseLines", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the last ADJUST base lines
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pType">STATIC or DYNAMIC or ALL (if "")</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 04/11/2014 BA-2065 (refactoring method GetLatestBaseLines in this class)
        ''' </remarks>
        Public Function GetCurrentAdjustBaseLineValuesByType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim blinesDelg As New WSBLinesDelegate
                        resultData = blinesDelg.GetCurrentBaseLineValues(dbConnection, pAnalyzerID, pType)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.GetCurrentAdjustBaseLineValuesByType", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Evaluate the adjust base line result and decide if are good enough or not
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pALineDS">DS must be sorted by Wavelength ASC</param>
        ''' <returns>GlobalDataTo with setDatos as GlobalEnumerates.Alarms</returns>
        ''' <remarks>
        ''' AG 02/05/2011 Creation
        ''' </remarks>
        Public Function ControlAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pALineDS As BaseLinesDS) As GlobalDataTO Implements IBaseLineEntity.ControlAdjustBaseLine
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        existsAlightResultsAttribute = True 'AG 20/06/2012
                        consecutiveRejectedWells = 0 'Clear the consecutive rejected wells counter every new ALIGHT results. Initialize counter
                        exitRunningTypeAttribute = 0

                        'Clear structure, fill it & evaluate results
                        validAlightAttribute = False
                        myListOfRejectedWells.Clear()

                        Dim resultsRejected As Boolean = False 'AG 12/09/2012 - move declaration out of the loop
                        With adjustBL
                            If Not .mainLight Is Nothing Then .mainLight.Clear() Else .mainLight = New List(Of Integer)
                            If Not .refLight Is Nothing Then .refLight.Clear() Else .refLight = New List(Of Integer)
                            If Not .mainDark Is Nothing Then .mainDark.Clear() Else .mainDark = New List(Of Integer)
                            If Not .refDark Is Nothing Then .refDark.Clear() Else .refDark = New List(Of Integer)
                            If Not .DAC Is Nothing Then .DAC.Clear() Else .DAC = New List(Of Single)
                            If Not .IT Is Nothing Then .IT.Clear() Else .IT = New List(Of Single)
                            If Not .enabled Is Nothing Then .enabled.Clear() Else .enabled = New List(Of Boolean)

                            .rejected = False

                            'Read the led positions and status
                            Dim ledPositionDelg As New AnalyzerLedPositionsDelegate
                            resultData = ledPositionDelg.GetAllWaveLengths(dbConnection, myAnalyzerID)
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                Dim myLedPositionsDS As New AnalyzerLedPositionsDS
                                myLedPositionsDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)

                                For Each row As AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow In myLedPositionsDS.tcfgAnalyzerLedPositions
                                    If Not row.IsStatusNull Then
                                        .enabled.Add(row.Status)
                                    Else
                                        .enabled.Add(False)
                                    End If
                                Next
                            End If

                            If Not resultData.HasError Then
                                Dim diodePosition As Integer = -1
                                'Dim resultsRejected As Boolean = False 'AG 12/09/2012 - move declaration out of the loop
                                For Each row As BaseLinesDS.twksWSBaseLinesRow In pALineDS.twksWSBaseLines
                                    If Not row.IsMainLightNull Then .mainLight.Add(row.MainLight) Else .mainLight.Add(0)
                                    If Not row.IsRefLightNull Then .refLight.Add(row.RefLight) Else .refLight.Add(0)
                                    If Not row.IsMainDarkNull Then .mainDark.Add(row.MainDark) Else .mainDark.Add(0)
                                    If Not row.IsRefLightNull Then .refDark.Add(row.RefDark) Else .refDark.Add(0)
                                    If Not row.IsDACNull Then .DAC.Add(row.DAC) Else .DAC.Add(0)
                                    If Not row.IsITNull Then .IT.Add(row.IT) Else .IT.Add(0)
                                    If Not row.IsWavelengthNull Then diodePosition = row.Wavelength - 1 Else diodePosition += 1

                                    If .enabled(diodePosition) Then
                                        'DAC control: DACref(i)/1.4 < DAC(i) < DACref(i)*2
                                        'Read adjutsments and evaluate limits
                                        Dim code As String = "ILED" 'ILED1 ... ILED11
                                        Dim myRes As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                                        myRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In instrumentAdjustments.srv_tfmwAdjustments _
                                                 Where String.Compare(a.CodeFw, code & (diodePosition + 1).ToString, False) = 0 Select a).ToList
                                        If myRes.Count > 0 Then
                                            If (CSng(myRes(0).Value) * DAC_LIMIT_MIN < row.DAC) Or (CSng(myRes(0).Value) * DAC_LIMIT_MAX > row.DAC) Then
                                                'rejected = True
                                            End If
                                        End If
                                        myRes = Nothing 'AG 25/02/2014 - #1521

                                        'Main light control
                                        If (MAINLIGHT_COUNTS_LIMIT_MIN > row.MainLight) Or (MAINLIGHT_COUNTS_LIMIT_MAX < row.MainLight) Then
                                            resultsRejected = True
                                        End If

                                        'Ref light control
                                        If (REFLIGHT_COUNTS_LIMIT_MIN > row.RefLight) Or (REFLIGHT_COUNTS_LIMIT_MAX < row.RefLight) Then
                                            resultsRejected = True
                                        End If

                                        'Main darkness control
                                        If (MAINDARK_COUNTS_LIMIT_MIN > row.MainDark) Or (MAINDARK_COUNTS_LIMIT_MAX < row.MainDark) Then
                                            resultsRejected = True
                                        End If

                                        'Ref darkness control
                                        If (REFDARK_COUNTS_LIMIT_MIN > row.RefDark) Or (REFDARK_COUNTS_LIMIT_MAX < row.RefDark) Then
                                            resultsRejected = True
                                        End If
                                        'diodePosition = diodePosition + 1

                                        If resultsRejected Then
                                            .rejected = True
                                            'Exit For
                                        End If
                                    End If

                                Next

                                'Initialize the well base line calculations control
                                InitStructures(True, True)

                                If resultsRejected Then 'AG 12/09/2012 - init structures clear the .Rejected value but we have to keep it
                                    .rejected = True
                                    'Exit For
                                End If

                                Dim myAlarm As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                                'rejected = False   'AG 18/05/2011 - comment after testing
                                If resultsRejected Then
                                    Dim myLogAcciones As New ApplicationLogManager()
                                    myLogAcciones.CreateLogActivity("ALIGHT Rejected", "BaseLineCalculations.ControlAdjustBaseLine", EventLogEntryType.Information, False) 'AG 14/05/2012 - Add more information
                                    myAlarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                                End If

                                If myAlarm = GlobalEnumerates.Alarms.NONE Then validAlightAttribute = True
                                resultData.SetDatos = myAlarm
                            End If

                        End With

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.ControlAdjustBaseLine", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Evaluate the well base line result and decide:
        ''' - Well is good or is rejected
        ''' - New reactions rotor is suggested
        ''' - New base line is suggested
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pClassInitialization" >FALSE: normal mode, TRUE: only on open the application. Do not save results into database due we
        ''' use data already saved</param>
        ''' <param name="pWellBaseLine" >we modify AbsValue and IsMean fields and the update table!!</param>
        ''' <returns>GlobalDataTO (GlobalEnumeates.Alarms)</returns>
        ''' <remarks>
        ''' AG 02/05/2011 Creation
        ''' </remarks>
        Public Function ControlWellBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pClassInitialization As Boolean, _
                                            ByVal pWellBaseLine As BaseLinesDS) As GlobalDataTO Implements IBaseLineEntity.ControlWellBaseLine
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                'AG 29/06/2012 - Running Cycles lost - Solution!
                'resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                'If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                '    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then

                Dim AnalyzerID As String = ""
                Dim WorkSessionID As String = ""
                Dim myBaseLineID As Integer = 0
                Dim myWellUsed As Integer = 0

                Dim calcDel As New CalculationsDelegate

                'Fill the structure for analyze the received well base line
                Dim diodePosition As Integer = -1 'array starts in 0
                Dim wellAlreadyExcluded As Boolean = False

                InitStructures(False, False)
                myListOfRejectedWells.Clear()

                'The firsts wells after ALIGHT results are not used for parameter initialization
                'Case running where Sw receives ANSPHR every cycle machine

                'AG 16/02/2012
                'If Not pClassInitialization And rejectionParameters.wellsNotUsedAfterALight <= BL_WELLREJECT_ITEMS_NOTUSED Then
                If rejectionParameters.wellsNotUsedAfterALight <= BL_WELLREJECT_ITEMS_NOTUSED Then
                    If Not pClassInitialization Then
                        rejectionParameters.wellsNotUsedAfterALight += 1
                    Else
                        'During initialization we only read the wells belongs to the MEAN so wellsNotUsedAfterALight can not taken into account
                        rejectionParameters.wellsNotUsedAfterALight = BL_WELLREJECT_ITEMS_NOTUSED + 1
                    End If

                End If

                For Each row As BaseLinesDS.twksWSBaseLinesRow In pWellBaseLine.twksWSBaseLines
                    With wellBL
                        If Not row.IsMainLightNull Then .mainLight.Add(row.MainLight) Else .mainLight.Add(0)
                        If Not row.IsRefLightNull Then .refLight.Add(row.RefLight) Else .refLight.Add(0)
                        If Not row.IsWavelengthNull Then diodePosition = row.Wavelength - 1 Else diodePosition += 1

                        .well = CInt(row.WellUsed)


                        'If diodePosition > -1 Then 'DL 03/10/2012

                        'Calculate well absorbance
                        If .mainLight(diodePosition) <> 0 And .refLight(diodePosition) <> 0 Then
                            resultData = calcDel.CalculateAbsorbance(.mainLight(diodePosition), .refLight(diodePosition), adjustBL.mainLight(diodePosition), _
                                                                      adjustBL.refLight(diodePosition), adjustBL.mainDark(diodePosition), adjustBL.refDark(diodePosition), _
                                                                       0, PATH_LENGHT, LIMIT_ABS, False)
                            row.BeginEdit()
                            row.IsMean = True
                            Dim resAbsorbance As Single
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                resAbsorbance = CType(resultData.SetDatos, Single)
                                .Abs.Add(resAbsorbance)
                                row.ABSvalue = resAbsorbance
                            Else
                                'NOTE: Same treatment when the abs is invalid or when a system error appears during abs calculation -> Reject well
                                resAbsorbance = (CType(resultData.SetDatos, Single))
                                .Abs.Add(resAbsorbance)

                                If adjustBL.enabled(diodePosition) Then
                                    row.ABSvalue = resAbsorbance
                                    .rejected = True
                                    row.IsMean = False
                                    If Not myListOfRejectedWells.Contains(.well) Then myListOfRejectedWells.Add(.well)
                                End If
                                resultData.HasError = False 'AG 13/02/2012 - CalculateAbsorbances activates HasError when invalid readings
                            End If

                            'Different business depending the algorithm mode (initialization or already initializated)
                            'If well is rejected for one wavelength do not analyze the others but acumulate into DS for save them
                            If Not InitializationComplete And Not wellAlreadyExcluded Then
                                If rejectionParameters.wellsNotUsedAfterALight > BL_WELLREJECT_ITEMS_NOTUSED Then
                                    InitialiazeWellRejectionControl(.well, resAbsorbance, diodePosition, row.IsMean)
                                    If .rejected Then
                                        wellAlreadyExcluded = True
                                        consecutiveRejectedWells += 1 'Increment counter
                                    End If

                                Else
                                    row.IsMean = False
                                End If
                            End If
                            row.EndEdit()
                        End If
                        'End If 'DL 03/10/2012

                    End With
                    'diodePosition += 1

                    If AnalyzerID = "" And Not row.IsAnalyzerIDNull Then AnalyzerID = row.AnalyzerID
                    If WorkSessionID = "" And Not row.IsWorkSessionIDNull Then WorkSessionID = row.WorkSessionID
                    If myBaseLineID = 0 And Not row.IsBaseLineIDNull Then myBaseLineID = row.BaseLineID
                    If Not row.IsWellUsedNull Then
                        If myWellUsed = 0 Or myWellUsed <> row.WellUsed Then
                            myWellUsed = row.WellUsed

                            'AG 16/02/2012 - Execute this condition before start the loops
                            ''During initialization we only read the wells belongs to the MEAN so wellsNotUsedAfterALight can not taken into account
                            'If pClassInitialization And rejectionParameters.wellsNotUsedAfterALight <= BL_WELLREJECT_ITEMS_NOTUSED Then
                            '    rejectionParameters.wellsNotUsedAfterALight = BL_WELLREJECT_ITEMS_NOTUSED + 1
                            'End If
                        End If
                    End If


                Next
                pWellBaseLine.AcceptChanges()

                'If results treat them
                Dim newReactionsWellsDS As New ReactionsRotorDS 'AG 12/06/2012
                If pWellBaseLine.twksWSBaseLines.Rows.Count > 0 Then
                    Dim processData As Boolean = False
                    Dim excludedWellFromFIFO As Integer = -1
                    Dim wellExcludedInitializationFromFIFO As String = ""

                    If rejectionParameters.wellsNotUsedAfterALight <= BL_WELLREJECT_ITEMS_NOTUSED Then
                        'Do nothing
                        If Not wellAlreadyExcluded Then consecutiveRejectedWells = 0 'Reset counter

                    ElseIf Not InitializationComplete Then
                        'AG 28/06/2012 - This line must be commented, the consecutiveRejectedWells counter is controled inside TreatInitializationResults method
                        'If Not wellAlreadyExcluded Then consecutiveRejectedWells = 0 'Reset counter 
                        wellExcludedInitializationFromFIFO = TreatInitializationResults(processData)

                    Else
                        'AG 05/06/2012 - This line must be commented, the consecutiveRejectedWells counter is controled inside TreatWellRejectionResults method
                        'If Not wellAlreadyExcluded Then consecutiveRejectedWells = 0 'Reset counter 
                        excludedWellFromFIFO = TreatWellRejectionResults()
                        processData = True
                    End If

                    Dim wellBLDelegate As New WSBLinesByWellDelegate 'Create twksWSBLinesByWell: ABSValue, IsMean (NULL)
                    Dim reactionsDelg As New ReactionsRotorDelegate 'Create twksWSReactionsRotor
                    Dim rejectedWells As String = ""
                    'Dim newReactionsWellsDS As New ReactionsRotorDS 'AG 12/06/2012

                    If Not pClassInitialization Then
                        'If well has been rejected create it with IsMean = False
                        If wellBL.rejected Then
                            For Each row As BaseLinesDS.twksWSBaseLinesRow In pWellBaseLine.twksWSBaseLines
                                row.BeginEdit()
                                row.IsMean = False
                                row.EndEdit()
                            Next
                            pWellBaseLine.AcceptChanges()
                        End If

                        resultData = wellBLDelegate.Create(dbConnection, pWellBaseLine)
                        If Not resultData.HasError Then
                            'Add records into table: twksWSReactionsRotor: for use or not the well
                            'If OK mark Status as Ready (R) and RejectedFlag = false, if rejected mark status as Rejected (X) and RejectedFlag = True

                            'AG 28/07/2011 - Add OrElse clause, it is required
                            'If myListOfRejectedWells.Contains(myWellUsed) Then rejectedWells = " " & myWellUsed & " "
                            If myListOfRejectedWells.Contains(myWellUsed) Or wellBL.rejected Then rejectedWells = " " & myWellUsed & " "
                            resultData = reactionsDelg.InitializeNewRotorTurnWellStatus(dbConnection, WorkSessionID, AnalyzerID, myWellUsed, rejectedWells, _
                                                                                        True, MAX_REACTROTOR_WELLS, WASHSTATION_BLW_LIMIT_MIN, WASHSTATION_BLW_LIMIT_MAX)

                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                newReactionsWellsDS = CType(resultData.SetDatos, ReactionsRotorDS)
                            End If
                        End If

                    End If

                    'Finally when intialization is finished and we are not openning the application update database
                    If Not pClassInitialization And processData Then
                        'Update table twksWSBLinesByWell: update field IsMean using myListOfRejectWells
                        If Not resultData.HasError Then
                            'Mark the excluded from FIFO well as not IsMean
                            'The current well is OK and enter into parameters region. Other has to leave it (FIFO)
                            If excludedWellFromFIFO <> -1 Then
                                resultData = wellBLDelegate.UpdateIsMean(dbConnection, AnalyzerID, WorkSessionID, myBaseLineID, False, excludedWellFromFIFO.ToString)
                            End If

                            If Not resultData.HasError Then
                                'Prepare the rejected wells list in string mode
                                'When the initialization fails all wells that form part of the initialization are rejected
                                rejectedWells = wellExcludedInitializationFromFIFO

                                'Some wells in initialization are rejected
                                For i As Integer = 0 To myListOfRejectedWells.Count - 1
                                    If rejectedWells = "" Then
                                        rejectedWells &= " " & myListOfRejectedWells(i).ToString & " "
                                    Else
                                        rejectedWells &= " , " & myListOfRejectedWells(i).ToString
                                    End If
                                Next

                                If rejectedWells <> "" Then
                                    resultData = wellBLDelegate.UpdateIsMean(dbConnection, AnalyzerID, WorkSessionID, myBaseLineID, False, rejectedWells)

                                    If Not resultData.HasError Then
                                        'if rejected mark status as Rejected (X) and RejectedFlag = True
                                        resultData = reactionsDelg.InitializeNewRotorTurnWellStatus(dbConnection, WorkSessionID, AnalyzerID, myWellUsed, rejectedWells, _
                                                                                                    False, MAX_REACTROTOR_WELLS, WASHSTATION_BLW_LIMIT_MIN, WASHSTATION_BLW_LIMIT_MAX)

                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            Dim auxReactionsWellDS As New ReactionsRotorDS
                                            auxReactionsWellDS = CType(resultData.SetDatos, ReactionsRotorDS)

                                            'Add records into the method returned variable (is called from 2 places)
                                            If auxReactionsWellDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                                For Each reactionRow As ReactionsRotorDS.twksWSReactionsRotorRow In auxReactionsWellDS.twksWSReactionsRotor.Rows
                                                    'newReactionsWellsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(reactionRow)
                                                    newReactionsWellsDS.twksWSReactionsRotor.ImportRow(reactionRow)
                                                Next
                                                newReactionsWellsDS.AcceptChanges()
                                            End If
                                        End If

                                    End If
                                End If

                                'Update the table tcfgAnalyzerReactionsRotor (BaseLine parameters rejected and wells rejected number in current rotor)
                                If Not resultData.HasError Then
                                    resultData = UpdateAnalyzerReactionsRotor(dbConnection)
                                End If

                            End If
                        End If

                    End If

                    'AG 12/06/2012
                    ''Finally raise event with the reactions rotor well new information
                    'If Not pClassInitialization And Not resultData.HasError And newReactionsWellsDS.twksWSReactionsRotor.Rows.Count > 0 Then
                    '    RaiseEvent WellReactionsChanges(newReactionsWellsDS)
                    'End If

                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    'If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)'AG 29/06/2012 - Running Cycles lost - Solution!

                    'rejectionParameters.alarm = GlobalEnumerates.Alarms.NONE 'AG 18/05/2011 - comment after testing
                    resultData.SetDatos = rejectionParameters.alarm

                    'AG 12/06/2012
                    'Finally raise event with the reactions rotor well new information
                    If Not pClassInitialization AndAlso Not resultData.HasError AndAlso newReactionsWellsDS.twksWSReactionsRotor.Rows.Count > 0 Then
                        RaiseEvent WellReactionsChanges(newReactionsWellsDS)
                    End If

                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    'If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)'AG 29/06/2012 - Running Cycles lost - Solution!
                End If

                'End If'AG 29/06/2012 - Running Cycles lost - Solution!
                'End If'AG 29/06/2012 - Running Cycles lost - Solution!

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                'If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)'AG 29/06/2012 - Running Cycles lost - Solution!

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.ControlWellBaseLine", EventLogEntryType.Error, False)
            Finally
                'If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()'AG 29/06/2012 - Running Cycles lost - Solution!
            End Try
            Return resultData
        End Function

        Public Sub ResetWS() Implements IBaseLineEntity.ResetWS
            InitStructures(True, False) 'Clear well base line parameters on RESET worksession
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Initialize() Implements IBaseLineEntity.Initialize
            Try
                'Field limits and Sw parameters variables (default value based on SPEC document but they are initialized in method GetClassParameterValues)
                'Adjust base line limits
                DAC_LIMIT_MIN = (1 / 1.4)
                DAC_LIMIT_MAX = 2
                MAINLIGHT_COUNTS_LIMIT_MIN = 600000
                MAINLIGHT_COUNTS_LIMIT_MAX = 930000
                REFLIGHT_COUNTS_LIMIT_MIN = 100000
                REFLIGHT_COUNTS_LIMIT_MAX = 930000
                MAINDARK_COUNTS_LIMIT_MIN = 3000
                MAINDARK_COUNTS_LIMIT_MAX = 6000
                REFDARK_COUNTS_LIMIT_MIN = 3000
                REFDARK_COUNTS_LIMIT_MAX = 6000
                BL_WELLREJECT_INI_SD = 0.027
                BL_WELLREJECT_INI_WELLNUMBER = 10

                'Well base line initialization process limits
                BL_WELLREJECT_INI_RANGE_LIMIT_MIN = -0.08
                BL_WELLREJECT_INI_RANGE_LIMIT_MAX = 0.08
                BL_WELLREJECT_INI_ABS_LIMIT_MIN = -0.08
                BL_WELLREJECT_INI_ABS_LIMIT_MAX = 0.08
                BL_WELLREJECTED_INI_LIMIT_MIN = 1
                BL_WELLREJECTED_INI_LIMIT_MAX = 3
                BL_WELLREJECT_ITEMS_NOTUSED = 7

                'Well base line (running) process limits
                BL_WELLREJECT_RANGE_LIMIT_MIN = -0.08
                BL_WELLREJECT_RANGE_LIMIT_MAX = 0.08
                BL_WELLREJECT_ABS_LIMIT_MIN = -0.08
                BL_WELLREJECT_ABS_LIMIT_MAX = 0.08
                BL_WELLREJECT_SD = 0.027

                PATH_LENGHT = 6.11
                LIMIT_ABS = 3.3
                MAX_REJECTED_WELLS = 20 'Max rejected wells allowed in one reactions rotor
                MAX_REACTROTOR_WELLS = 120 'Max wells in reactions rotor

                WASHSTATION_BLW_LIMIT_MIN = -7 'BLW value - 7 ... BLW value: wells in washing station
                WASHSTATION_BLW_LIMIT_MAX = 3 'BLW value ... BLW value + 3 : wells in washing station

                myAnalyzerID = ""

                fieldLimitsAttribute = New FieldLimitsDS
                swParametersAttribute = New ParametersDS
                instrumentAdjustments = New SRVAdjustmentsDS
                validAlightAttribute = False
                existsAlightResultsAttribute = False 'AG 20/06/2012

                adjustBL = New AdjustBaseLine
                wellBL = New wellBaseLine
                rejectionParameters = New initializationParameters
                InitializationComplete = False
                myListOfRejectedWells = New List(Of Integer)

                BL_CONSECUTIVEREJECTED_WELL = 30
                consecutiveRejectedWells = 0
                exitRunningTypeAttribute = 0

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Private Base Line Calculations Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pWell"></param>
        ''' <param name="pWellAbsorbance"></param>
        ''' <param name="pDiodePosition"></param>
        ''' <param name="pAddToRejectionParameters" ></param>
        ''' <remarks></remarks>
        Private Sub InitialiazeWellRejectionControl(ByVal pWell As Integer, ByVal pWellAbsorbance As Single, ByVal pDiodePosition As Integer, ByVal pAddToRejectionParameters As Boolean)
            Try

                If pAddToRejectionParameters Then

                    With rejectionParameters
                        'Add well used, increment initialization item and flags into average control
                        If Not .wellUsed.Contains(pWell) Then
                            .wellUsed.Add(pWell)
                            .rejected.Add(False)
                            .initializationParameterItem += 1
                        End If

                        '1st in Initialization mode: Add absorbance to average control
                        If .absByWELL Is Nothing Then
                            ReDim Preserve .absByWELL(pDiodePosition)
                            .absByWELL(pDiodePosition).Abs = New List(Of Single)
                        ElseIf .absByWELL.Length - 1 < pDiodePosition Then
                            ReDim Preserve .absByWELL(pDiodePosition)
                            .absByWELL(pDiodePosition).Abs = New List(Of Single)
                        End If
                        .absByWELL(pDiodePosition).Abs.Add(pWellAbsorbance)

                    End With

                Else
                    'The well is discarted due some diode position has absorbance error
                    'Remove it from rejectionparameters structure
                    If pDiodePosition > 0 Then
                        With rejectionParameters
                            If .wellUsed.Contains(pWell) Then
                                .wellUsed.RemoveRange(.wellUsed.Count - 1, 1)
                                .rejected.RemoveRange(.rejected.Count - 1, 1)
                                If .initializationParameterItem > 0 Then .initializationParameterItem -= 1
                            End If

                            If .absByWELL Is Nothing Then
                                'No items ... do nothing
                            ElseIf .absByWELL(0).Abs.Count = 1 Then
                                'Only one item ... remove it
                                Erase .absByWELL
                            Else
                                'Several items (wells) in struture ... Remove only current wellBL
                                For wl As Integer = 0 To pDiodePosition - 1
                                    .absByWELL(wl).Abs.RemoveRange(0, 1)
                                Next

                            End If
                        End With
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.InitializationWellRejectionControl", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pProcessData" ></param>
        ''' <remarks></remarks>
        Private Function TreatInitializationResults(ByRef pProcessData As Boolean) As String
            Dim wellsExcludedFromFIFO As String = ""
            Try
                ''Work with a temporal structure
                'Dim auxResults As New initializationParameters
                'auxResults = rejectionParameters
                'With auxResults

                With rejectionParameters
                    Dim infoRejection As String = "" 'AG 14/05/2012
                    If .initializationParameterItem >= BL_WELLREJECT_INI_WELLNUMBER Then
                        InitializationComplete = True
                        pProcessData = True

                        '1st step: For each wavelenght and for each abs used in initialization determine how many are rejected
                        Dim myAbsAverage As Single = 0
                        Dim rejectedWellsNumber As Integer = myListOfRejectedWells.Count
                        For wl As Integer = 0 To .absByWELL.Length - 1
                            myAbsAverage = .absByWELL(wl).Abs.Average

                            If adjustBL.enabled(wl) Then 'Check only the active leds
                                For wellId As Integer = 0 To .absByWELL(wl).Abs.Count - 1
                                    'AG 08/01/2014 - Sometimes an out of range exception is launched here
                                    'If Not .rejected(wellId) Then
                                    If wellId <= .rejected.Count - 1 AndAlso Not .rejected(wellId) Then
                                        If (.absByWELL(wl).Abs(wellId) < myAbsAverage + BL_WELLREJECT_INI_RANGE_LIMIT_MIN) Or _
                                            (.absByWELL(wl).Abs(wellId) > myAbsAverage + BL_WELLREJECT_INI_RANGE_LIMIT_MAX) Then

                                            rejectedWellsNumber += 1 'Increment the counter of rejected wells
                                            .rejected(wellId) = True 'Mark well as rejected

                                            If Not myListOfRejectedWells.Contains(.wellUsed(wellId)) Then myListOfRejectedWells.Add(.wellUsed(wellId))
                                        End If

                                    End If
                                Next
                            End If

                        Next

                        '2on step: Evaluate results
                        If rejectedWellsNumber >= BL_WELLREJECTED_INI_LIMIT_MAX Then
                            'Reject initialization and show message: Check reactions rotor
                            .initRejected = True

                        ElseIf rejectedWellsNumber >= BL_WELLREJECTED_INI_LIMIT_MIN Then
                            'Exclude from structure wells marked as rejected and recalculate absorbance Avg
                            For wellId As Integer = .absByWELL(0).Abs.Count - 1 To 0 Step -1
                                If .rejected.Count >= wellId Then
                                    If .rejected(wellId) Then
                                        .rejected.RemoveRange(wellId, 1)
                                        .wellUsed.RemoveRange(wellId, 1)
                                        For wl As Integer = 0 To .absByWELL.Length - 1
                                            .absByWELL(wl).Abs.RemoveRange(wellId, 1)
                                        Next
                                        '.initItem -= 1
                                    End If
                                End If
                            Next

                            'Repeat 1st step without the rejected wells
                            rejectedWellsNumber = 0
                            For wl As Integer = 0 To .absByWELL.Length - 1
                                myAbsAverage = .absByWELL(wl).Abs.Average

                                If adjustBL.enabled(wl) Then 'Check only the active leds
                                    For wellId As Integer = 0 To .absByWELL(wl).Abs.Count - 1
                                        If (.absByWELL(wl).Abs(wellId) < myAbsAverage + BL_WELLREJECT_INI_RANGE_LIMIT_MIN) Or _
                                           (.absByWELL(wl).Abs(wellId) > myAbsAverage + BL_WELLREJECT_INI_RANGE_LIMIT_MAX) Then

                                            rejectedWellsNumber += 1 'Increment the counter of rejected wells
                                            .rejected(wellId) = True 'Mark well as rejected

                                            If Not myListOfRejectedWells.Contains(.wellUsed(wellId)) Then myListOfRejectedWells.Add(.wellUsed(wellId))
                                        End If
                                    Next
                                End If

                            Next

                            If rejectedWellsNumber >= BL_WELLREJECTED_INI_LIMIT_MIN Then
                                'Reject initialization and show message: Check reactions rotor
                                .initRejected = True
                                infoRejection = "Rejected wells number (" & rejectedWellsNumber & ") > Limit (" & BL_WELLREJECTED_INI_LIMIT_MIN & ")" 'AG 14/05/2012
                            End If

                        End If


                        If Not .initRejected Then
                            Dim existError As Boolean = False
                            .absAVG.Clear()
                            .absSD.Clear()
                            Dim myUtil As New Utilities
                            For wl As Integer = 0 To .absByWELL.Length - 1
                                .absAVG.Add(.absByWELL(wl).Abs.Average)
                                .absSD.Add(myUtil.CalculateStandardDeviation(.absByWELL(wl).Abs))

                                If adjustBL.enabled(wl) Then 'Check only the active leds
                                    If (.absAVG(wl) < BL_WELLREJECT_INI_ABS_LIMIT_MIN) Or (.absAVG(wl) > BL_WELLREJECT_INI_ABS_LIMIT_MAX) Then existError = True
                                    If .absSD(wl) > BL_WELLREJECT_INI_SD Then existError = True
                                    If existError Then
                                        infoRejection = "Wavelenght ID: " & wl + 1 & ", Avg Abs(wavelenght): " & .absAVG(wl) & ", SD(wavelenght): " & .absSD(wl) 'AG 14/05/2012 (AG 17/09/2012 add 1 to the wavelenght ID)
                                        Exit For
                                    End If

                                End If
                            Next

                            If existError Then
                                .initRejected = True
                            End If

                        End If

                        ''Temporal CODE for testings - Leave commented
                        '.initRejected = True

                        If .initRejected Then
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("Well rejection initialization parameters (FIFO) rejected. " & infoRejection, "BaseLineCalculations.TreatInitializationResults", EventLogEntryType.Information, False) 'AG 14/05/2012 - Add more information

                            'If initialization rejected then ... update variable wellsExcludedFromFIFO Then
                            For i As Integer = 0 To .wellUsed.Count - 1
                                If String.Compare(wellsExcludedFromFIFO, "", False) = 0 Then
                                    wellsExcludedFromFIFO &= " " & .wellUsed(i).ToString & " "
                                Else
                                    wellsExcludedFromFIFO &= " , " & .wellUsed(i).ToString & " "
                                End If
                            Next

                            InitializationComplete = False
                            InitStructures(True, False)
                            'AG 14/10/2013 - Fix issue #1320 (baseline error appears when initialization fails 3 consecutive times)
                            '.alarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                            consecutiveRejectedWells += BL_WELLREJECT_INI_WELLNUMBER 'Increment counter (all wells inside parameters FIFO are rejected)

                        Else
                            If exitRunningTypeAttribute = 0 Then
                                consecutiveRejectedWells = 0 'Initializatio phase accepted, clear the consecutive rejected wells counter. Initilize counter
                            End If

                        End If

                    End If

                    If consecutiveRejectedWells >= BL_CONSECUTIVEREJECTED_WELL Then
                        If exitRunningTypeAttribute = 0 Then
                            .alarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                            exitRunningTypeAttribute = 2
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("Max consecutive initializations rejected limit!!", "BaseLineCalculations.TreatInitializationResults", EventLogEntryType.Information, False)
                        End If
                    End If

                End With


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.TreatInitializationResults", EventLogEntryType.Error, False)
            End Try
            Return wellsExcludedFromFIFO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>Integer indicating the excluded well from rejection parameters (if -1 no excluded wells)</returns>
        ''' <remarks>
        ''' AG 26/09/2012 - infoRejection string get new value only if current value is ""</remarks>
        Private Function TreatWellRejectionResults() As Integer
            Dim myExcludedWell As Integer = -1
            Try
                With rejectionParameters
                    Dim infoRejection As String = "" 'AG 14/05/2012
                    'Check if the new well absorbance are inside limits for each wavelenght (else reject it)
                    For wl As Integer = 0 To .absByWELL.Length - 1
                        If Not wellBL.rejected Then
                            If adjustBL.enabled(wl) Then 'Check only the active leds
                                If (wellBL.Abs(wl) < .absAVG(wl) + BL_WELLREJECT_INI_RANGE_LIMIT_MIN) Or _
                                    (wellBL.Abs(wl) > .absAVG(wl) + BL_WELLREJECT_INI_RANGE_LIMIT_MAX) Then

                                    wellBL.rejected = True
                                    'If Not myListOfRejectedWells.Contains(wellBL.well) Then myListOfRejectedWells.Add(wellBL.well)
                                    If wellBL.rejected AndAlso String.Compare(infoRejection, "", False) = 0 Then 'AG 26/09/2012
                                        infoRejection = "Well: " & wellBL.well & ", Wavelenght ID: " & wl + 1 & ", Avg Abs(wavelenght): " & .absAVG(wl) & ", SD(wavelenght): " & .absSD(wl) 'AG 14/05/2012 (AG 17/09/2012 add 1 to the wavelenght ID)
                                    End If

                                End If

                            End If
                        End If
                    Next

                    ''Temporal CODE for testings - Leave commented
                    'wellBL.rejected = True

                    If Not wellBL.rejected Then
                        If exitRunningTypeAttribute = 0 Then
                            consecutiveRejectedWells = 0 'Well accepted, clear the consecutive rejected wells counter. Initialize counter
                        End If

                        infoRejection = ""
                        'If well OK then add into rejection parameters structure and update it (rejectionParameters is a FIFO)
                        'Add well used, increment initialization item and flags into average control
                        If Not .wellUsed.Contains(wellBL.well) Then
                            .wellUsed.Add(wellBL.well)
                            .rejected.Add(False)
                            .initializationParameterItem += 1
                        End If

                        'Add the new well absorbances (last position) and remove the first item in list
                        For wl As Integer = 0 To .absByWELL.Length - 1
                            .absByWELL(wl).Abs.Add(wellBL.Abs(wl))
                            .absByWELL(wl).Abs.RemoveRange(0, 1)
                        Next
                        myExcludedWell = .wellUsed(0) 'The well leaving the rejection parameters
                        .initializationParameterItem -= 1
                        .rejected.RemoveRange(0, 1)
                        .wellUsed.RemoveRange(0, 1)

                        'Recalculate average and standard deviation
                        Dim warnMethacrylate As Boolean = False

                        .absAVG.Clear()
                        .absSD.Clear()
                        Dim myUtil As New Utilities
                        For wl As Integer = 0 To .absByWELL.Length - 1
                            .absAVG.Add(.absByWELL(wl).Abs.Average)
                            .absSD.Add(myUtil.CalculateStandardDeviation(.absByWELL(wl).Abs))

                            If adjustBL.enabled(wl) Then 'Check only the active leds
                                If (.absAVG(wl) < BL_WELLREJECT_ABS_LIMIT_MIN) Or (.absAVG(wl) > BL_WELLREJECT_ABS_LIMIT_MAX) Then warnMethacrylate = True
                                If .absSD(wl) > BL_WELLREJECT_SD Then warnMethacrylate = True
                                'If warnMethacrylate Then Exit For
                                If warnMethacrylate AndAlso String.Compare(infoRejection, "", False) = 0 Then 'AG 26/09/2012
                                    infoRejection = "Wavelenght ID: " & wl + 1 & ", Avg Abs(wavelenght): " & .absAVG(wl) & ", SD(wavelenght): " & .absSD(wl) 'AG 14/05/2012 (AG 17/09/2012 add 1 to the wavelenght ID)
                                End If

                            End If
                        Next

                        If warnMethacrylate Then
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("Update wells rejection parameters (FIFO) rejected. " & infoRejection, "BaseLineCalculations.TreatWellRejectionResults", EventLogEntryType.Information, False) 'AG 14/05/2012 - Add more information

                            .alarm = GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                            '.initRejected = True '????
                        End If
                    Else
                        'Well rejected
                        Dim myLogAcciones As New ApplicationLogManager()
                        myLogAcciones.CreateLogActivity("Well rejected. " & infoRejection, "BaseLineCalculations.TreatWellRejectionResults", EventLogEntryType.Information, False) 'AG 14/05/2012 - Add more information

                        'AG 04/06/2012
                        consecutiveRejectedWells += 1 'Increment counter
                        If consecutiveRejectedWells >= BL_CONSECUTIVEREJECTED_WELL Then
                            If exitRunningTypeAttribute = 0 Then
                                .alarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                                exitRunningTypeAttribute = 1
                                myLogAcciones.CreateLogActivity("Max consecutive rejected wells limit!!", "BaseLineCalculations.TreatWellRejectionResults", EventLogEntryType.Information, False)
                            End If
                        End If
                        'AG 04/06/2012
                    End If

                End With

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.TreatWellRejectionResults", EventLogEntryType.Error, False)
            End Try
            Return myExcludedWell
        End Function


        ''' <summary>
        ''' Prepare data and inform about the current reactions rotor status installed in Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UpdateAnalyzerReactionsRotor(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Determine is create or update (this table contains one record for each analyzer)
                        Dim createFlag As Boolean = True
                        Dim myAnReactRotorDelg As New AnalyzerReactionsRotorDelegate
                        Dim myDS As New AnalyzerReactionsRotorDS
                        Dim row As AnalyzerReactionsRotorDS.tcfgAnalyzerReactionsRotorRow

                        resultData = myAnReactRotorDelg.Read(dbConnection, myAnalyzerID)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDS = CType(resultData.SetDatos, AnalyzerReactionsRotorDS)
                            If myDS.tcfgAnalyzerReactionsRotor.Rows.Count > 0 Then createFlag = False

                            If createFlag Then
                                row = myDS.tcfgAnalyzerReactionsRotor.NewtcfgAnalyzerReactionsRotorRow
                            Else
                                row = myDS.tcfgAnalyzerReactionsRotor(0)
                            End If

                            Dim wellRejectedCount As Integer = 0
                            With row
                                .BeginEdit()
                                .AnalyzerID = myAnalyzerID

                                If createFlag Then .InstallDate = Now Else .SetInstallDateNull()

                                Dim reactionsRotorDel As New ReactionsRotorDelegate
                                resultData = reactionsRotorDel.GetAllWellsLastTurn(dbConnection, myAnalyzerID)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    Dim reactionsRotorDS As New ReactionsRotorDS
                                    reactionsRotorDS = CType(resultData.SetDatos, ReactionsRotorDS)

                                    Dim rejectedWellsList As List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                                    rejectedWellsList = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In reactionsRotorDS.twksWSReactionsRotor _
                                                         Where a.RejectedFlag = True Select a).ToList
                                    row.WellsRejectedNumber = rejectedWellsList.Count
                                    rejectedWellsList = Nothing 'AG 25/02/2014 - #1521
                                End If
                                wellRejectedCount = row.WellsRejectedNumber
                                row.EndEdit()

                                'Evaluate if alarm or not (current rejected wells > limit)
                                'AG 07/09/2012 - add not baseline error condition
                                If Not resultData.HasError AndAlso rejectionParameters.alarm <> GlobalEnumerates.Alarms.BASELINE_INIT_ERR _
                                   AndAlso wellRejectedCount > MAX_REJECTED_WELLS Then
                                    'If Not resultData.HasError Andalso wellRejectedCount > MAX_REJECTED_WELLS Then

                                    rejectionParameters.alarm = GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                                End If

                                If rejectionParameters.alarm <> GlobalEnumerates.Alarms.NONE Then
                                    .BLParametersRejected = True
                                Else
                                    .BLParametersRejected = False
                                End If

                            End With

                            If createFlag Then myDS.tcfgAnalyzerReactionsRotor.AddtcfgAnalyzerReactionsRotorRow(row)
                            myDS.AcceptChanges()

                            If createFlag Then
                                resultData = myAnReactRotorDelg.Create(dbConnection, myDS)
                            Else
                                resultData = myAnReactRotorDelg.Update(dbConnection, myDS)
                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.UpdateAnalyzerReactionsRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


#End Region

#Region "Other private methods"

        ''' <summary>
        ''' Initialize the well rejection control structure
        ''' </summary>
        ''' <param name="pResetWellRejectionParameters" ></param>
        ''' <remarks>
        ''' AG 03/05/2011 Creation
        ''' </remarks>
        Private Sub InitStructures(ByVal pResetWellRejectionParameters As Boolean, ByVal pNewAdjustBaseLine As Boolean)
            Try
                With wellBL
                    If Not .mainLight Is Nothing Then .mainLight.Clear() Else .mainLight = New List(Of Integer)
                    If Not .refLight Is Nothing Then .refLight.Clear() Else .refLight = New List(Of Integer)
                    If Not .Abs Is Nothing Then .Abs.Clear() Else .Abs = New List(Of Single)
                    .well = 0
                    .rejected = False
                End With

                If pResetWellRejectionParameters Then
                    With rejectionParameters
                        If Not .absAVG Is Nothing Then .absAVG.Clear() Else .absAVG = New List(Of Single)
                        If Not .absSD Is Nothing Then .absSD.Clear() Else .absSD = New List(Of Single)
                        Erase .absByWELL
                        If Not .wellUsed Is Nothing Then .wellUsed.Clear() Else .wellUsed = New List(Of Integer)
                        If Not .rejected Is Nothing Then .rejected.Clear() Else .rejected = New List(Of Boolean)

                        .alarm = GlobalEnumerates.Alarms.NONE
                        .initializationParameterItem = 0
                        .initRejected = False

                        If pNewAdjustBaseLine Then .wellsNotUsedAfterALight = 0 'After an ALIGHT results the firsts BL wells must be not used
                    End With

                    InitializationComplete = False

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.InitStructures", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Initialize the internal variables using preloaded limits using FieldLimits or SwParameters tables
        ''' </summary>
        ''' <param name="pUseFieldLimitTableFlag"></param>
        ''' <remarks>
        ''' AG 03/05/2011 creation
        ''' </remarks>
        Private Sub GetClassParameterValues(ByVal pUseFieldLimitTableFlag As Boolean)
            Try
                If pUseFieldLimitTableFlag Then
                    'Adjust BL parameter limits
                    Dim qLingLimits As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)
                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                  Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.DAC_LIMIT.ToString, False) = 0 Select a).ToList
                    If qLingLimits.Count > 0 Then
                        DAC_LIMIT_MIN = qLingLimits(0).MinValue
                        DAC_LIMIT_MAX = qLingLimits(0).MaxValue
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where String.Compare(a.LimitID, GlobalEnumerates.FieldLimitsEnum.MAINLIGHT_COUNTS_LIMIT.ToString, False) = 0 Select a).ToList
                    If qLingLimits.Count > 0 Then
                        MAINLIGHT_COUNTS_LIMIT_MIN = CInt(qLingLimits(0).MinValue)
                        MAINLIGHT_COUNTS_LIMIT_MAX = CInt(qLingLimits(0).MaxValue)
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.REFLIGHT_COUNTS_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        REFLIGHT_COUNTS_LIMIT_MIN = CInt(qLingLimits(0).MinValue)
                        REFLIGHT_COUNTS_LIMIT_MAX = CInt(qLingLimits(0).MaxValue)
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.MAINDARK_COUNTS_LIMIT.ToString Select a).ToList

                    If qLingLimits.Count > 0 Then
                        MAINDARK_COUNTS_LIMIT_MIN = CInt(qLingLimits(0).MinValue)
                        MAINDARK_COUNTS_LIMIT_MAX = CInt(qLingLimits(0).MaxValue)
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.REFDARK_COUNTS_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        REFDARK_COUNTS_LIMIT_MIN = CInt(qLingLimits(0).MinValue)
                        REFDARK_COUNTS_LIMIT_MAX = CInt(qLingLimits(0).MaxValue)
                    End If

                    'Well base line initialization process limits
                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.BL_WELLREJECT_INI_RANGE_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        BL_WELLREJECT_INI_RANGE_LIMIT_MIN = qLingLimits(0).MinValue
                        BL_WELLREJECT_INI_RANGE_LIMIT_MAX = qLingLimits(0).MaxValue
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.BL_WELLREJECT_INI_ABS_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        BL_WELLREJECT_INI_ABS_LIMIT_MIN = qLingLimits(0).MinValue
                        BL_WELLREJECT_INI_ABS_LIMIT_MAX = qLingLimits(0).MaxValue
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.BL_WELLREJECTED_INI_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        BL_WELLREJECTED_INI_LIMIT_MIN = CInt(qLingLimits(0).MinValue)
                        BL_WELLREJECTED_INI_LIMIT_MAX = CInt(qLingLimits(0).MaxValue)
                    End If

                    'Well base line (running) process limits
                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.BL_WELLREJECT_RANGE_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        BL_WELLREJECT_RANGE_LIMIT_MIN = qLingLimits(0).MinValue
                        BL_WELLREJECT_RANGE_LIMIT_MAX = qLingLimits(0).MaxValue
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.BL_WELLREJECT_ABS_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        BL_WELLREJECT_ABS_LIMIT_MIN = qLingLimits(0).MinValue
                        BL_WELLREJECT_ABS_LIMIT_MAX = qLingLimits(0).MaxValue
                    End If

                    qLingLimits = (From a As FieldLimitsDS.tfmwFieldLimitsRow In fieldLimitsAttribute.tfmwFieldLimits _
                                   Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.WASHSTATION_BLW_LIMIT.ToString Select a).ToList
                    If qLingLimits.Count > 0 Then
                        WASHSTATION_BLW_LIMIT_MIN = CInt(qLingLimits(0).MinValue)
                        WASHSTATION_BLW_LIMIT_MAX = CInt(qLingLimits(0).MaxValue)
                    End If



                Else
                    'Well base line initialization process limits
                    Dim qLingParameter As New List(Of ParametersDS.tfmwSwParametersRow)
                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                  Where a.ParameterName = GlobalEnumerates.SwParameters.BL_WELLREJECT_INI_SD.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        BL_WELLREJECT_INI_SD = qLingParameter(0).ValueNumeric
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.BL_WELLREJECT_INI_WELLNUMBER.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        BL_WELLREJECT_INI_WELLNUMBER = CInt(qLingParameter(0).ValueNumeric)
                    End If

                    'Well base line (running) process limits
                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.BL_WELLREJECT_SD.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        BL_WELLREJECT_SD = qLingParameter(0).ValueNumeric
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.PATH_LENGHT.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        PATH_LENGHT = qLingParameter(0).ValueNumeric
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.LIMIT_ABS.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        LIMIT_ABS = qLingParameter(0).ValueNumeric
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.MAX_REJECTED_WELLS.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        MAX_REJECTED_WELLS = CInt(qLingParameter(0).ValueNumeric)
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                    Where a.ParameterName = GlobalEnumerates.SwParameters.MAX_REACTROTOR_WELLS.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        MAX_REACTROTOR_WELLS = CInt(qLingParameter(0).ValueNumeric)
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.BL_WELLREJECT_ITEMS_NOTUSED.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        BL_WELLREJECT_ITEMS_NOTUSED = CInt(qLingParameter(0).ValueNumeric)
                    End If

                    qLingParameter = (From a As ParametersDS.tfmwSwParametersRow In swParametersAttribute.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.BL_CONSECUTIVEREJECTED_WELL.ToString Select a).ToList
                    If qLingParameter.Count > 0 Then
                        BL_CONSECUTIVEREJECTED_WELL = CInt(qLingParameter(0).ValueNumeric)
                    End If
                End If


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.GetClassParameterValues", EventLogEntryType.Error, False)
            End Try

        End Sub


        ' ''' <summary>
        ' ''' Calculate standar deviation for a list of single values
        ' ''' </summary>
        ' ''' <param name="pCollection"></param>
        ' ''' <returns>SD as single</returns>
        ' ''' <remarks>AG 03/05/2011 creation</remarks>
        'Private Function CalculateStandardDeviation(ByVal pCollection As List(Of Single)) As Single
        '    Dim myResult As Single = 0
        '    Try
        '        ''Formula initial: WRONG
        '        'Dim myAverage As Single = pCollection.Average
        '        'Dim items As Integer = pCollection.Count

        '        'Dim sum As Single = 0
        '        'For i As Integer = 0 To pCollection.Count - 1
        '        '    sum += (pCollection(i) - myAverage)
        '        'Next

        '        'If items > 0 Then
        '        '    sum = sum / items
        '        'End If

        '        'If sum > 0 Then
        '        '    myResult = CSng(Math.Sqrt(CDbl(sum)))
        '        'End If

        '        'Formula final: OK (STortosa 06/07/2011)
        '        Dim myAverage As Single = pCollection.Average
        '        Dim items As Integer = pCollection.Count - 1

        '        Dim sum As Single = 0
        '        For i As Integer = 0 To pCollection.Count - 1
        '            sum += CSng((pCollection(i) - myAverage) ^ 2)
        '        Next

        '        If items > 0 Then
        '            sum = sum / items
        '        End If

        '        If sum > 0 Then
        '            myResult = CSng(Math.Sqrt(CDbl(sum)))
        '        End If

        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "BaseLineCalculations.CalculateStandardDeviation", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResult

        'End Function

#End Region

    End Class

End Namespace

