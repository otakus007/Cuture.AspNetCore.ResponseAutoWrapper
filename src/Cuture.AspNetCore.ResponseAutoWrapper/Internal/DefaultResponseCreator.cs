﻿using System.Runtime.CompilerServices;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal
{
    /// <summary>
    /// 默认的 <inheritdoc cref="IResponseCreator{TResponse}"/>
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    internal class DefaultResponseCreator<TResponse> : IResponseCreator<TResponse>
        where TResponse : class, new()
    {
        //OPT build as create delegate for type

        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResponse Create(int code, string? message = null)
        {
            var response = new TResponse();

            (response as ISetResponseCode)?.SetCode(code);
            (response as ISetResponseMessage)?.SetMessage(message);

            return response;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResponse Create(int code, object? data, string? message = null)
        {
            var response = new TResponse();

            (response as ISetResponseCode)?.SetCode(code);
            (response as ISetResponseMessage)?.SetMessage(message);
            (response as ISetResponseData)?.SetData(data);

            return response;
        }

        #endregion Public 方法
    }
}