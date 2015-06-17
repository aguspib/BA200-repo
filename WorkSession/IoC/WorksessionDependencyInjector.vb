Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Types

Public Module WSDependencyInjector
    ''' <summary>
    ''' This is a dependency injection target to solve cross referencing between Ax00.Core.Entities. ....
    ''' </summary>
    ''' <remarks></remarks>
    Public WSCreator As IWSExecutionCreator

    ''' <summary>
    ''' This is a pointer to the ContaminationsManager constructor.
    ''' This pointer will be accessing the appropriate method in a superior business layer
    ''' </summary>
    ''' <remarks></remarks>
    Public ContaminationsManagerConstructor As Func(Of Boolean, Integer, ContaminationsDS, 
                                                    List(Of ExecutionsDS.twksWSExecutionsRow), 
                                                    List(Of Integer), List(Of Integer), 
                                                    IContaminationManager)
End Module
