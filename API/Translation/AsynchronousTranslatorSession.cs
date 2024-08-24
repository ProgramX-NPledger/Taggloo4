﻿using API.Data;
using API.Hubs;
using API.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;

namespace API.Translation;

/// <summary>
/// Provides a single entry-point for translation services, for web client and API clients.
/// </summary>
public class AsynchronousTranslatorSession : ITranslatorSession
{
    private readonly TranslatorOptions _options;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    private readonly DataContext _entityFrameworkCoreDataContext;
    private readonly IWebHostEnvironment _webHosEnvironment;
    // private readonly ICompositeViewEngine _compositeViewEngine;
    // private readonly ITempDataProvider _tempDataProvider;
    // private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor using injected objects for Hangfire and SignalR services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of the Hangfire <seealso cref="IBackgroundJobClient"/> object.</param>
    /// <param name="hubContext">Implementation of the SignalR <seealso cref="IHubContext{TranslateHub}"/> object.</param>
    public AsynchronousTranslatorSession(IBackgroundJobClient backgroundJobClient, 
        IHubContext<TranslateHub> hubContext, 
        DataContext entityFrameworkCoreDataContext,
        // ICompositeViewEngine compositeViewEngine,
        // ITempDataProvider tempDataProvider,
        // IHttpContextAccessor httpContextAccessor
        IWebHostEnvironment webHosEnvironment) 
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _entityFrameworkCoreDataContext = entityFrameworkCoreDataContext;
        _webHosEnvironment = webHosEnvironment;
        // _compositeViewEngine = compositeViewEngine;
        // _tempDataProvider = tempDataProvider;
        // _httpContextAccessor = httpContextAccessor;
        _options = new TranslatorOptions();
    }

    /// <summary>
    /// Constructor with additional options opportunities.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of the Hangfire <seealso cref="IBackgroundJobClient"/> object.</param>
    /// <param name="hubContext">Implementation of the SignalR <seealso cref="IHubContext{TranslateHub}"/> object.</param>
    /// <param name="options">Options which may customise translation behaviour.</param>
    public AsynchronousTranslatorSession(IBackgroundJobClient backgroundJobClient, 
        IHubContext<TranslateHub> hubContext, 
        DataContext entityFrameworkCoreDataContext, 
        // ICompositeViewEngine compositeViewEngine,
        // ITempDataProvider tempDataProvider,
        // IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHosEnvironment,
        TranslatorOptions options) : this(backgroundJobClient, hubContext, entityFrameworkCoreDataContext,webHosEnvironment)
    {
        _options = options;
    }

    /// <summary>
    /// Invokes a Translation Job using the Hangfire Background Job Client.
    /// </summary>
    /// <param name="translationRequest">The <see cref="TranslationRequest"/> representing the translation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the state of the object has not been correctly initialised.</exception>
    public void Translate(TranslationRequest translationRequest)
    {
        if (_backgroundJobClient == null)
            throw new InvalidOperationException("BackgroundJobClient implementation cannot be null");
        if (_hubContext == null)
            throw new InvalidOperationException($"{nameof(IHubContext)} implementation cannot be null");
        
        // create the hangfire job and submit
        TranslateJob translateJob = new TranslateJob(_backgroundJobClient, _hubContext, _entityFrameworkCoreDataContext, _webHosEnvironment);
        translateJob.AddTranslationJob(translationRequest);
    }
}