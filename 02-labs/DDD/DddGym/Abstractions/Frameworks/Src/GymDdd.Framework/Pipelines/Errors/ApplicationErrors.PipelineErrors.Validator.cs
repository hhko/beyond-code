﻿using GymDdd.Framework.BaseTypes;
using LanguageExt.Common;

namespace GymDdd.Framework.Pipelines.Errors;

public static partial class ApplicationErrors
{
    public static partial class PipelineErrors
    {
        public static Error Validator(string propertyName, string errorMessage) =>
            ErrorCodeFactory.Create(
                $"{nameof(ApplicationErrors)}.{nameof(PipelineErrors)}.{nameof(Validator)}",
                $"{propertyName}: {errorMessage}");
    }
}
