''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Enum BarcodeWorksessionActionsEnum
    BARCODE_AVAILABLE = 0 'Barcode has not pending work to performe
    NO_RUNNING_REQUEST = 1 'User press read barcode from RotorPosition screen
    BEFORE_RUNNING_REQUEST = 2 'User press START o CONTINUE worksession button
    REAGENTS_REQUEST_BEFORE_RUNNING = 3 'After (1) ... the Reagents barcode request has been sent. Waiting for results
    SAMPLES_REQUEST_BEFORE_RUNNING = 4 'After (2) ... the Samples barcode request has been sent. Waiting for results
    ENTER_RUNNING = 5 'Barcode read management performed ... ready to enter in Running
    FORCE_ENTER_RUNNING = 6 'AG 08/03/2013 - Force enter running although exists samples without requests assigned
    FORCE_ENTER_RUNNING_WITHOUT_CREATE_EXECUTIONS = 7 'AG 01/04/2014 - #1565 Force enter running when autoWSCreation process finishes adding new rquests to WS, in this case the executions have already created, it is not necessary created them again
End Enum
