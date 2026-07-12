#!/usr/bin/env dotnet-script
// If you have any questions about this file, check out https://github.com/StateSmith/tutorial-2
#r "nuget: StateSmith, 0.22.2"

using StateSmith.Common;
using StateSmith.Input.Expansions;
using StateSmith.Output.UserConfig;
using StateSmith.Runner;
using StateSmith.SmGraph;

// See https://github.com/StateSmith/tutorial-2/tree/main/lesson-1
SmRunner runner = new(diagramPath: "SyncServer.plantuml", new MyRenderConfig(), transpilerId: TranspilerId.CSharp);
AddEnterAndExitCalls();
runner.Run();

void AddEnterAndExitCalls() {
    runner.SmTransformer.InsertBeforeFirstMatch(StandardSmTransformer.TransformationId.Standard_Validation1, (sm) => {
    sm.VisitTypeRecursively((State s) => {
        if (s.Behaviors.Any(b => b.ToString() == "enter")) {
            s.AddEnterAction($"On{s.Name}Enter();", 0);
        } else if (s.Behaviors.Any(b => b.ToString() == "exit")) {
            s.AddExitAction($"On{s.Name}Exit();", 0);
        }
    });
});
}

// See https://github.com/StateSmith/tutorial-2/tree/main/lesson-2/ (basics)
// See https://github.com/StateSmith/tutorial-2/tree/main/lesson-5/ (language specific options)
public class MyRenderConfig : IRenderConfigCSharp
{
    string IRenderConfigCSharp.NameSpace => "FinancialTracker.StateMachines;";

    bool IRenderConfigCSharp.UsePartialClass => true;

    // See https://github.com/StateSmith/tutorial-2/tree/main/lesson-3
    public class MyExpansions : UserExpansionScriptBase
    {
        // See https://github.com/StateSmith/tutorial-2/tree/main/lesson-4 for timing expansions
    }
}
