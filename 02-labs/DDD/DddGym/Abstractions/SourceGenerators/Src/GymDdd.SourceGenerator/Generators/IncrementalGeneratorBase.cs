﻿using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;

namespace GymDdd.SourceGenerator.Generators;

public abstract class IncrementalGeneratorBase<TValue>(
    Func<IncrementalGeneratorInitializationContext, IncrementalValuesProvider<TValue>> registerSourceProvider,
    Action<SourceProductionContext, ImmutableArray<TValue>> generate,
    //Action<IncrementalGeneratorPostInitializationContext>? registerPostInitializationSourceOutput = null,
    bool AttachDebugger = false) : IIncrementalGenerator
{
    protected const string ClassEntityName = "class";
    //protected const string MethodEntityName = "method";
    //protected const string PropertyEntityName = "property";
    //protected const string FieldEntityName = "field";

    private readonly bool _attachDebugger = AttachDebugger;
    private readonly Func<IncrementalGeneratorInitializationContext, IncrementalValuesProvider<TValue>> _registerSourceProvider = registerSourceProvider;
    private readonly Action<SourceProductionContext, ImmutableArray<TValue>> _generate = generate;
    //private readonly Action<IncrementalGeneratorPostInitializationContext>? _registerPostInitializationSourceOutput = registerPostInitializationSourceOutput;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (_attachDebugger && Debugger.IsAttached is false)
        {
            Debugger.Launch();
        }

        //if (_registerPostInitializationSourceOutput is not null)
        //{
        //    context.RegisterPostInitializationOutput(_registerPostInitializationSourceOutput);
        //}

        IncrementalValuesProvider<TValue> provider = _registerSourceProvider(context)
            //.WithTrackingName(TrackingNames.InitialValues)
            .Where(static m => m is not null);
        //.WithTrackingName(TrackingNames.NotNullValues);

        context.RegisterSourceOutput(provider.Collect(), Execute);
    }

    private void Execute(SourceProductionContext context, ImmutableArray<TValue> displayValues)
    {
        //// 생성할 클래스가 없을 때: 경고 메시지
        //if (displayValues.Length is 0)
        //{
        //    ReportNoValueFound(
        //        context,
        //        ClassEntityName,
        //        $"No {ClassEntityName} declared in the actual scope. ");
        //}

        // 소스 생성
        _generate(context, displayValues);

        //void ReportNoValueFound(SourceProductionContext context, string entityName, string warning)
        //{
        //    var diagnosticDescription = new DiagnosticDescriptor
        //    (
        //        "SG0001",
        //        $"No {entityName} Found",
        //        warning,
        //        "Problem",
        //        DiagnosticSeverity.Warning,
        //        true
        //    );

        //    context.ReportDiagnostic(Diagnostic.Create(diagnosticDescription, Location.None));
        //}
    }
}
