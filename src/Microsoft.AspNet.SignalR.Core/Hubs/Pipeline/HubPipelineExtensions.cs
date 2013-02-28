// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using Microsoft.AspNet.SignalR.Compression;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR
{
    public static class HubPipelineExtensions
    {
        /// <summary>
        /// Requiring Authentication adds an <see cref="AuthorizeModule"/> to the <see cref="IHubPipeline" /> with <see cref="IAuthorizeHubConnection"/>
        /// and <see cref="IAuthorizeHubMethodInvocation"/> authorizers that will be applied globally to all hubs and hub methods.
        /// These authorizers require that the <see cref="System.Security.Principal.IPrincipal"/>'s <see cref="System.Security.Principal.IIdentity"/> 
        /// IsAuthenticated for any clients that invoke server-side hub methods or receive client-side hub method invocations. 
        /// </summary>
        /// <param name="pipeline">The <see cref="IHubPipeline" /> to which the <see cref="AuthorizeModule" /> will be added.</param>
        public static void RequireAuthentication(this IHubPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            var authorizer = new AuthorizeAttribute();
            pipeline.AddModule(new AuthorizeModule(globalConnectionAuthorizer: authorizer, globalInvocationAuthorizer: authorizer));
        }

        public static void CompressPayloads(this IHubPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException("pipeline");
            }

            var resolver = GlobalHost.DependencyResolver;

            var payloadDescriptorProvider = new Lazy<ReflectedPayloadDescriptorProvider>(() => new ReflectedPayloadDescriptorProvider(resolver));
            resolver.Register(typeof(IPayloadDescriptorProvider), () => payloadDescriptorProvider.Value);

            var payloadCompressor = new Lazy<DefaultPayloadCompressor>(() => new DefaultPayloadCompressor(resolver));
            resolver.Register(typeof(IPayloadCompressor), () => payloadCompressor.Value);

            var payloadDecompressor = new Lazy<DefaultPayloadDecompressor>(() => new DefaultPayloadDecompressor(resolver));
            resolver.Register(typeof(IPayloadDecompressor), () => payloadDecompressor.Value);

            var contractGenerator = new Lazy<DefaultContractsGenerator>(() => new DefaultContractsGenerator(resolver));
            resolver.Register(typeof(IContractsGenerator), () => contractGenerator.Value);

            var parameterBinder = new Lazy<CompressableParameterResolver>(() => new CompressableParameterResolver(payloadDescriptorProvider.Value, payloadDecompressor.Value));
            resolver.Register(typeof(IParameterResolver), () => parameterBinder.Value);

            pipeline.AddModule(new PayloadCompressionModule(resolver.Resolve<IPayloadCompressor>(), resolver.Resolve<IPayloadDescriptorProvider>()));
        }
    }
}
