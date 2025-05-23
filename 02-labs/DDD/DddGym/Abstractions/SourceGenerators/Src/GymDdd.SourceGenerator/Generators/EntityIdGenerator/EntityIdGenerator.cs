﻿using GymDdd.SourceGenerator.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using static GymDdd.SourceGenerator.Abstractions.Constants;

namespace GymDdd.SourceGenerator.Generators.EntityIdGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class EntityIdGenerator()
    : IncrementalGeneratorBase<EntityIdToGenerateEntry>(
        RegisterSourceProvider,
        Generate)
{
    private const string GenerateEntityIdAttributeMetadataName = "System.GenerateEntityIdAttribute";
    private const string GenerateEntityIdAttributeFileName = "GenerateEntityIdAttribute.g.cs";

    public const string GenerateEntityIdAttribute = Header + """

namespace System;

/// <summary>
/// Add to entities to indicate that entity id structure should be generated
/// </summary>
[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Generated by source generator.")]
public class GenerateEntityIdAttribute : global::System.Attribute;
""";

    private static IncrementalValuesProvider<EntityIdToGenerateEntry> RegisterSourceProvider(
        IncrementalGeneratorInitializationContext context)
    {
        //
        // 1단계: 고정 코드 생성 (Post-initialization)
        //  - 소스 코드 분석과 무관하게 항상 생성되는 코드 등록 (예: Attribute 정의)
        //  - 컴파일 초기 단계에 한 번만 실행됨
        //
        // Source Generator가 실행될 때 가장 먼저 [GenerateEntityId]라는 커스텀 속성(Attribute)을 생성하도록 합니다.
        // 이 코드는 실제 소스 코드에 해당 속성이 없어도 컴파일러가 이 속성을 인식하게 해 줍니다.
        //
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource(
                hintName: GenerateEntityIdAttributeFileName,                        // 생성될 파일 이름 (예: "GenerateEntityIdAttribute.g.cs")
        sourceText: SourceText.From(GenerateEntityIdAttribute, Encoding.UTF8)));    // 파일에 포함될 코드 내용 (예: [GenerateEntityId] 특성 정의)

        //
        // 2단계: 대상 필터링 및 코드 생성 준비
        //  - [GenerateEntityId] 같은 특성이 붙은 "클래스"만 대상으로 삼음
        //  - 이후 코드 생성에 필요한 정보 구조로 변환
        //
        // [GenerateEntityId] 속성이 있는 클래스를 EntityIdToGenerateEntry과 매핑합니다.
        //
        return context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: GenerateEntityIdAttributeMetadataName,  // System.GenerateEntityIdAttribute
                predicate: Selectors.IsClass,                                       // 클래스 선언인지 확인 (예: class Foo { })
                transform: MapToEntityIdToGenerate)                                 // 클래스 → 소스 생성 입력 모델로 변환
        .Where(x => x != EntityIdToGenerateEntry.None);                          // 변환 실패 or 무시할 항목은 필터링
    }

    private static EntityIdToGenerateEntry MapToEntityIdToGenerate(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        // 클래스가 없을 때
        if (context.TargetSymbol is not INamedTypeSymbol entitySymbol)
        {
            return EntityIdToGenerateEntry.None;
        }

        cancellationToken.ThrowIfCancellationRequested();

        // 클래스가 있을 때

        // 클래스 이름
        string name = entitySymbol.Name + "Id";
        // 클래스 네임스페이스
        string @namespace = entitySymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : entitySymbol.ContainingNamespace.ToString();

        return new EntityIdToGenerateEntry(name, @namespace);
    }

    // 매핑된 EntityIdToGenerateEntry로부터 소스 파일을 생성합니다.
    private static void Generate(SourceProductionContext context, ImmutableArray<EntityIdToGenerateEntry> entityIdToGenerateEntries)
    {
        foreach (var entityIdToGenerateEntry in entityIdToGenerateEntries)
        {
            StringBuilder sb = new();
            string source = entityIdToGenerateEntry.Generate(sb);   // 파일 소스 생성
            context.AddSource(
                entityIdToGenerateEntry.Name + ".g.cs",             // 생성할 파일 이름
                SourceText.From(source, Encoding.UTF8));            // 생성할 파일 소스
        }
    }
}
