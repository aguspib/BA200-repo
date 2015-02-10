Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.PresentationCOM

Namespace RotorUtils
    ''' <summary>
    ''' This module will contain all shared functionality between UiMonitor and IWSRotorPositions
    ''' </summary>
    ''' <remarks></remarks>
    Module ReagentExpirationParsing

        ''' <summary>
        ''' Get the Expiration date from the reagent barcode information.
        ''' </summary>
        ''' <param name="pReagentBarcode"></param>
        ''' <returns>Valid datepart in pReagentBarcode ==> Expiration Date.
        '''          Datepart in pReagentBarcode represents invalid date ==> Date.MinValue</returns>
        ''' <remarks>
        ''' Created by:  TR 28/03/2014
        ''' Modified by: TR 10/04/2014 bt #1583-Initialize the ExpirationDate variable to min Date value.
        '''              XB 10/07/2014 - DateTime to Invariant Format (MM dd yyyy) - Bug #1673
        '''              WE 07/10/2014 - Extend code with check on Month field to prevent String to Date Conversion Error shown on screen (BA-1965).
        '''              MI 26/01/2015 - Made Shared so it can be used in any other class.
        ''' </remarks>
        Public Function GetReagentExpDateFromBarCode(pReagentBarcode As String, F As BSBaseForm) As Date
            Dim ExpirationDate As Date = Date.MinValue
            Try
                ExpirationDate = ParseReagentExpDateFromBarCode(pReagentBarcode)
                Return ExpirationDate
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                F.ShowMessage(F.Name & ".GetReagentExpDateFromBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", F)
            End Try
            Return ExpirationDate
        End Function

        ''' <summary>
        ''' This function parses the expiration date for a Reagent into a DateTime instance.
        ''' </summary>
        ''' <param name="pReagentBarCode"></param>
        ''' <returns>the expiration date</returns>
        ''' <remarks></remarks>
        Private Function ParseReagentExpDateFromBarCode(pReagentBarCode As String) As Date
            Dim ExpirationDate As Date = Date.MinValue
            Try
                Dim myMonth As String = ""
                Dim myYear As String = ""
                If pReagentBarCode <> "" Then
                    'The month start on position 6 to 7 (2pos)
                    myMonth = pReagentBarCode.Substring(5, 2)
                    'The year start on position 8 to 9 (2pos)
                    myYear = pReagentBarCode.Substring(7, 2)
                    'Add to year expiration the 2000 to avoid error of 1900
                    myYear = "20" & myYear

                    'Set the result value.
                    'If myMonth <> "" OrElse myYear <> "" Then
                    If myMonth <> "" AndAlso myYear <> "" AndAlso CInt(myMonth) >= 1 AndAlso CInt(myMonth) <= 12 Then
                        ' XB 10/07/2014 - DateTime to Invariant Format - Bug #1673
                        'Date.TryParse("01" & "-" & myMonth & "-" & myYear, ExpirationDate)
                        'ExpirationDate = CDate(myMonth & "-" & "01" & "-" & myYear).ToString(CultureInfo.InvariantCulture)
                        ExpirationDate = New DateTime(CInt(myYear), CInt(myMonth), 1)
                    End If
                End If
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex) 'ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetReagentExpDateFromBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                Throw ex
                'ShowMessage(Me.Name & ".GetReagentExpDateFromBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            End Try
            Return ExpirationDate

        End Function
    End Module
End Namespace