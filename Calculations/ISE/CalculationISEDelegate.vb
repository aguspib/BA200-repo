Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.Calculations
    Public Class CalculationISEDelegate

        ''' <summary>
        ''' Apply the correction calculation to the recived concentration. 
        ''' calculation formula ==> Y= SlopeA * x + SlopeB.
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
        ''' </remarks>
        Public Function CalculateConcentrationCorrection(ByVal pSampleType As String, ByVal pElectrodeID As String, _
                                                         ByVal pConcValue As Single) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim mySlopeA As Single = 0
                Dim mySlopeB As Single = 0

                'Get Slopes values.
                Select Case pElectrodeID
                    Case "Li"
                        'If Not String.Equals(pSampleType, "URI") Then  ' XB 04/06/2013
                        If Not pSampleType.Contains("URI") Then         ' XB 04/06/2013
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_SER_Li)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_Li)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)
                        End If

                    Case "Na"
                        'If Not String.Equals(pSampleType, "URI") Then  ' XB 04/06/2013
                        If Not pSampleType.Contains("URI") Then         ' XB 04/06/2013
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_SER_Na)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_Na)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)

                        Else
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_URI_Na)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_Na)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)

                        End If

                    Case "K"
                        'If pSampleType <> "URI" Then               ' XB 04/06/2013
                        If Not pSampleType.Contains("URI") Then     ' XB 04/06/2013
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_SER_K)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_K)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)

                        Else
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_URI_K)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_K)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)

                        End If
                    Case "Cl"
                        'If pSampleType <> "URI" Then               ' XB 04/06/2013
                        If Not pSampleType.Contains("URI") Then     ' XB 04/06/2013
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_SER_Cl)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_SER_Cl)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)

                        Else
                            'Get Slope value 
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_SLOPE_URI_Cl)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeA = DirectCast(myGlobalDataTO.SetDatos, Single)

                            'Get Intercept value
                            myGlobalDataTO = GetSlopeValues(GlobalEnumerates.SwParameters.ISE_INTERCEPT_URI_Cl)
                            If myGlobalDataTO.HasError Then Exit Select
                            mySlopeB = DirectCast(myGlobalDataTO.SetDatos, Single)

                        End If
                End Select

                Dim myNewConcValue As Single = 0

                'NOTE: EF & AG 09/11/2012 Urine ISE results (require dilutions) but Sw do NOT apply the predilution factor to the concentration result (as we do in biochemical)
                '                    the own ISE module knows it is an urine sample and apply automatically the correct factor
                '                    Sw has to apply only the slope correction


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
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationISEDelegate.CalculateConcentrationCorrection", EventLogEntryType.Error, False)
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


    End Class
End Namespace
