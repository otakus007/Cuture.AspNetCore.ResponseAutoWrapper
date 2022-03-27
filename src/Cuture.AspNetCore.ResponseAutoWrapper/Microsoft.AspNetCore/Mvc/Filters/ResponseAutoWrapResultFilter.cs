﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace Microsoft.AspNetCore.Mvc.Filters;

internal class ResponseAutoWrapResultFilter<TResponse> : IAsyncAlwaysRunResultFilter
    where TResponse : class
{
    #region Private 字段

    private readonly IActionResultWrapper<TResponse> _actionResultWrapper;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="ResponseAutoWrapResultFilter{TResponse}"/>
    public ResponseAutoWrapResultFilter(IActionResultWrapper<TResponse> actionResultWrapper)
    {
        _actionResultWrapper = actionResultWrapper ?? throw new ArgumentNullException(nameof(actionResultWrapper));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (GetActionResultPolicy(context) != ActionResultPolicy.Skip
            && _actionResultWrapper.Wrap(context) is TResponse response)
        {
            context.Result = new OkObjectResult(response);
        }

        await next();
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ActionResultPolicy GetActionResultPolicy(ResultExecutingContext context)
    {
        //HACK 不加锁读取 ActionDescriptor.Properties ，可能存在某些问题
        if (context.ActionDescriptor.Properties.TryGetValue(Constants.ActionPropertiesResultPolicyKey, out var cachedPolicy))
        {
            return (ActionResultPolicy)cachedPolicy!;
        }

        //HACK 理论上不应该执行到这里
        Debug.WriteLine("Warning!!! Not found Constants.ActionPropertiesResultPolicyKey in ActionDescriptor.Properties");

        return ActionResultPolicy.Unknown;
    }

    #endregion Private 方法
}