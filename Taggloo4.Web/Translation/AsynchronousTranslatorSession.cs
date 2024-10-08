﻿using Hangfire;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Taggloo4.Contract;
using Taggloo4.Contract.Translation;
using Taggloo4.Data.EntityFrameworkCore;
using Taggloo4.Translation;
using Taggloo4.Web.Contract;
using Taggloo4.Web.Data;
using Taggloo4.Web.Hangfire.Jobs;
using Taggloo4.Web.Hubs;

namespace Taggloo4.Web.Translation;

/// <summary>
/// Provides a single entry-point for translation services, for web client and Taggloo4.Web clients.
/// </summary>
public class AsynchronousTranslatorSession : ITranslatorSession
{
    private readonly TranslatorOptions _options;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IHubContext<TranslateHub> _hubContext;
    private readonly DataContext _entityFrameworkCoreDataContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TranslatorConfigurationCache _translatorConfigurationCache;
    private readonly ITranslatorConfigurationRepository _translatorConfigurationRepository;

    /// <summary>
    /// Constructor using injected objects for Hangfire and SignalR services.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of the Hangfire <seealso cref="IBackgroundJobClient"/> object.</param>
    /// <param name="hubContext">Implementation of the SignalR <seealso cref="IHubContext{TranslateHub}"/> object.</param>
    /// <param name="entityFrameworkCoreDataContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="webHostEnvironment">Implementation of ASP.NET <seealso cref="IWebHostEnvironment"/>.</param>
    /// <param name="translatorConfigurationCache">Translator configuration cache.</param>
    /// <param name="translatorConfigurationRepository">Implementation of <seealso cref="ITranslatorConfigurationRepository"/>.</param>
    public AsynchronousTranslatorSession(IBackgroundJobClient backgroundJobClient, 
        IHubContext<TranslateHub> hubContext, 
        DataContext entityFrameworkCoreDataContext,
        IWebHostEnvironment webHostEnvironment,
        TranslatorConfigurationCache translatorConfigurationCache,
        ITranslatorConfigurationRepository translatorConfigurationRepository) 
    {
        _backgroundJobClient = backgroundJobClient;
        _hubContext = hubContext;
        _entityFrameworkCoreDataContext = entityFrameworkCoreDataContext;
        _webHostEnvironment = webHostEnvironment;
        _translatorConfigurationCache = translatorConfigurationCache;
        _translatorConfigurationRepository = translatorConfigurationRepository;
        _options = new TranslatorOptions();
    }

    /// <summary>
    /// Constructor with additional options opportunities.
    /// </summary>
    /// <param name="backgroundJobClient">Implementation of the Hangfire <seealso cref="IBackgroundJobClient"/> object.</param>
    /// <param name="hubContext">Implementation of the SignalR <seealso cref="IHubContext{TranslateHub}"/> object.</param>
    /// <param name="entityFrameworkCoreDataContext">Entity Framework context to enable access to underlying datastore.</param>
    /// <param name="webHostEnvironment">Implementation of ASP.NET <seealso cref="IWebHostEnvironment"/>.</param>
    /// <param name="translatorConfigurationCache">Translator configuration cache.</param>
    /// <param name="translatorConfigurationRepository">Implementation of <seealso cref="ITranslatorConfigurationRepository"/>.</param>
    /// <param name="options">Options which may customise translation behaviour.</param>
    public AsynchronousTranslatorSession(IBackgroundJobClient backgroundJobClient, 
        IHubContext<TranslateHub> hubContext, 
        DataContext entityFrameworkCoreDataContext, 
        IWebHostEnvironment webHostEnvironment,
        TranslatorConfigurationCache translatorConfigurationCache,
        ITranslatorConfigurationRepository translatorConfigurationRepository,
        TranslatorOptions options) : this(backgroundJobClient, hubContext, entityFrameworkCoreDataContext,webHostEnvironment,translatorConfigurationCache,translatorConfigurationRepository)
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
        TranslateJob translateJob = new TranslateJob(_backgroundJobClient, _hubContext, _entityFrameworkCoreDataContext, _webHostEnvironment, _translatorConfigurationCache, _translatorConfigurationRepository);
        translateJob.AddTranslationJob(translationRequest);
    }
}