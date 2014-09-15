Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.Calculations
    Public Class CalculationISEDelegate

        ''' <summary>
        ''' Apply the preloaded correction calculation to the recived concentration. 
        ''' calculation formula ==> Y= SlopeA1 * x + SlopeB1.
        ''' </summary>
        ''' <param name="pSampleType"></param>
        ''' <param name="pElectrodeID"></param>
        ''' <param name="pConcValue"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:  TR 14/03/2012
        ''' MODIFIED BY: TR 13/04/2012 - On the sample type validation instead of validating the SER and URI, we only validate if SampleType is
        '''                              diferent then URI, because the Samples type are PLM, SER, URI. (by now)
        '''              XB 04/06/2013 - change URI sample type comparisons in order to prepare the code in front of DB changes
        '''              AG 15/09/2014 - BA-1918 apply new preloaded ISE correction factors (for URI - Li and for PLM)
        '''                              Rewrite the code and rename method from CalculateConcentrationCorrection to CalculatePreloadedConcentrationCorrection
        '''                              to differentiate from the optional user defined correction
        ''' </remarks>
        Public Function CalculatePreloadedConcentrationCorrection(ByVal pSampleType As String, ByVal pElectrodeID As String, _
                                                         ByVal pConcValue As Single) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim mySlopeA As Single = 1
                Dim mySlopeB As Single = 0

                Dim myParamA As GlobalEnumerates.SwParameters = GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS 'Initiate with a wrong value
                Dim myParamB As GlobalEnumerates.SwParameters = GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS 'Initiate with a wrong value

                'Get Slopes values.
                'AG 15/09/2014 - BA-1918 different slope factors for SER, PLM, URI
                Select Case pElectrodeID
                    Case "Li"
                        If pSampleType.Contains("SER") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_SER_Li
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_Li

                        Else
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_URI_Li
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_Li
                        End If

                    Case "Na"
                        If pSampleType.Contains("SER") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_SER_Na
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_Na

                        ElseIf pSampleType.Contains("PLM") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_PLM_Na
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_PLM_Na

                        Else 'URI
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_URI_Na
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_Na
                        End If

                    Case "K"
                        If pSampleType.Contains("SER") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_SER_K
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_K

                        ElseIf pSampleType.Contains("PLM") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_PLM_K
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_PLM_K

                        Else 'URI
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_URI_K
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_K
                        End If

                    Case "Cl"
                        If pSampleType.Contains("SER") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_SER_Cl
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_Cl

                        ElseIf pSampleType.Contains("PLM") Then
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_PLM_Cl
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_PLM_Cl

                        Else 'URI
                            myParamA = GlobalEnumerates.SwParameters.ISE_SLOPE_URI_Cl
                            myParamB = GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_Cl
                        End If

                End Select

                'Get Slope values
                If myParamA <> GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS AndAlso myParamB <> GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS Then
                    myGlobalDataTO = GetSlopeValues(myParamA)
                    If Not myGlobalDataTO.HasError Then
                        mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                        'Get Intercept value
                        myGlobalDataTO = GetSlopeValues(myParamB)
                        If Not myGlobalDataTO.HasError Then
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)
                        End If
                    End If
                End If
                'AG 15/09/2014 - BA-1918

                'Dim myNewConcValue As Single = 0
                Dim myNewConcValue As Single = pConcValue

                'Formula Y= SlopeA * x + SlopeB
                'Validate if not error.
                If Not myGlobalDataTO.HasError Then
                    myNewConcValue = (mySlopeA * pConcValue) + mySlopeB
                End If

                myGlobalDataTO.SetDatos = myNewConcValue

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationISEDelegate.CalculatePreloadedConcentrationCorrection", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the slope parameter value
        ''' </summary>
        ''' <param name="pParameterName"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 14/03/2012</remarks>
        Private Function GetSlopeValues(ByVal pParameterName As GlobalEnumerates.SwParameters) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim mySWParametersDS As New ParametersDS
                Dim mySWParametersDelegate As New SwParametersDelegate
                Dim myParameterValue As Single = 0
                myGlobalDataTO = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, "", pParameterName.ToString(), False)
                If Not myGlobalDataTO.HasError Then
                    mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                    If mySWParametersDS.tfmwSwParameters.Count > 0 Then
                        myParameterValue = mySWParametersDS.tfmwSwParameters(0).ValueNumeric
                    End If
                End If

                myGlobalDataTO.SetDatos = myParameterValue

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationISEDelegate.GetSlopeValues", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Apply the user defined correction calculation to the recived concentration. 
        ''' calculation formula ==> Y= SlopeA2 * x + SlopeB2
        ''' </summary>
        ''' <param name="pSampleType"></param>
        ''' <param name="pISETestID"></param>
        ''' <param name="pConcValue"></param>
        ''' <returns>GlobaldataTO with single as data </returns>
        ''' <remarks>
        ''' CREATED BY:  AG 15/09/2014 - BA-1918
        ''' </remarks>
        Public Function CalculateUserDefinedConcentrationCorrection(ByVal pSampleType As String, ByVal pISETestID As Integer, _
                                                         ByVal pConcValue As Single) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim mySlopeA As Single = 1
                Dim mySlopeB As Single = 0
                Dim userSlopeDefined As Boolean = False

                'Read tparISETestSamples by pSampleType , pISETestID
                'If results found update mySlopeA, mySlopeB variables
                Dim testSamplesDlg As New ISETestSamplesDelegate
                myGlobalDataTO = testSamplesDlg.GetListByISETestID(Nothing, pISETestID, pSampleType)
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    Dim auxDS As New ISETestSamplesDS
                    auxDS = DirectCast(myGlobalDataTO.SetDatos, ISETestSamplesDS)
                    If auxDS.tparISETestSamples.Rows.Count > 0 AndAlso Not auxDS.tparISETestSamples(0).IsSlopeFactorA2Null AndAlso Not auxDS.tparISETestSamples(0).IsSlopeFactorB2Null Then
                        mySlopeA = auxDS.tparISETestSamples(0).SlopeFactorA2
                        mySlopeB = auxDS.tparISETestSamples(0).SlopeFactorB2
                        UserSlopeDefined = True
                    End If
                End If

                Dim myNewConcValue As Single = pConcValue

                'Formula Y= SlopeA * x + SlopeB
                'Validate if not error.
                If Not myGlobalDataTO.HasError AndAlso UserSlopeDefined Then
                    myNewConcValue = (mySlopeA * pConcValue) + mySlopeB
                End If

                myGlobalDataTO.SetDatos = myNewConcValue

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationISEDelegate.CalculateUserDefinedConcentrationCorrection", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

    End Class
End Namespace
