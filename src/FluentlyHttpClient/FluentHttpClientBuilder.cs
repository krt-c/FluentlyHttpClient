﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FluentlyHttpClient
{
	/// <summary>
	/// Class to configure <see cref="FluentHttpClient"/> with a fluent API.
	/// </summary>
	public class FluentHttpClientBuilder
	{
		/// <summary>
		/// Gets the identifier specified.
		/// </summary>
		public string Identifier { get; private set; }

		private readonly FluentHttpClientFactory _fluentHttpClientFactory;
		private string _baseUrl;
		private TimeSpan _timeout;
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
		private readonly List<Type> _middleware = new List<Type>();
		private Action<FluentHttpRequestBuilder> _requestBuilderDefaults;
		private HttpMessageHandler _httpMessageHandler;

		public FluentHttpClientBuilder(FluentHttpClientFactory fluentHttpClientFactory)
		{
			_fluentHttpClientFactory = fluentHttpClientFactory;
		}

		/// <summary>
		/// Set base url for each request.
		/// </summary>
		/// <param name="url">Base url address to set. e.g. "https://api.sketch7.com"</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithBaseUrl(string url)
		{
			_baseUrl = url;
			return this;
		}

		/// <summary>
		/// Sets the timespan to wait before the request times out (in seconds).
		/// </summary>
		/// <param name="timeout">Timeout to set (in seconds).</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithTimeout(int timeout) => WithTimeout(TimeSpan.FromSeconds(timeout));

		/// <summary>
		/// Sets the timespan to wait before the request times out (in seconds).
		/// </summary>
		/// <param name="timeout">Timeout to set.</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithTimeout(TimeSpan timeout)
		{
			_timeout = timeout;
			return this;
		}

		/// <summary>
		/// Add the specified header and its value for each request.
		/// </summary>
		/// <param name="key">Header to add.</param>
		/// <param name="value">Value for the header.</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithHeader(string key, string value)
		{
			if (_headers.ContainsKey(key))
				_headers[key] = value;
			else
				_headers.Add(key, value);
			return this;
		}

		/// <summary>
		/// Add the specified headers and their value for each request.
		/// </summary>
		/// <param name="headers">Headers to add.</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithHeaders(IDictionary<string, string> headers)
		{
			foreach (var item in headers)
				WithHeader(item.Key, item.Value);
			return this;
		}

		/// <summary>
		/// Set the identifier (unique key) for the Http Client.
		/// </summary>
		/// <param name="identifier">Identifier to set.</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder Withdentifier(string identifier)
		{
			Identifier = identifier;
			return this;
		}

		/// <summary>
		/// Add a handler which allows to customize the <see cref="FluentHttpRequestBuilder"/> on <see cref="FluentHttpClient.CreateRequest"/>.
		/// In order to specify defaults as desired, or so.
		/// </summary>
		/// <param name="requestBuilderDefaults">Action which pass <see cref="FluentHttpRequestBuilder"/> for customization.</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithRequestBuilderDefaults(Action<FluentHttpRequestBuilder> requestBuilderDefaults)
		{
			_requestBuilderDefaults = requestBuilderDefaults;
			return this;
		}

		/// <summary>
		/// Set HTTP handler stack to use for sending requests.
		/// </summary>
		/// <param name="handler">HTTP handler to use.</param>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder WithMessageHandler(HttpMessageHandler handler)
		{
			_httpMessageHandler = handler;
			return this;
		}

		/// <summary>
		/// Register middleware for the HttpClient, which each request pass-through. <c>NOTE order matters</c>.
		/// </summary>
		/// <typeparam name="T">Middleware type.</typeparam>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder AddMiddleware<T>() => AddMiddleware(typeof(T));

		/// <summary>
		/// Register middleware for the HttpClient, which each request pass-through. <c>NOTE order matters</c>.
		/// </summary>
		/// <returns>Returns client builder for chaining.</returns>
		public FluentHttpClientBuilder AddMiddleware(Type middleware)
		{
			_middleware.Add(middleware);
			return this;
		}

		/// <summary>
		/// Build up http client options.
		/// </summary>
		/// <returns>Returns http client options.</returns>
		public FluentHttpClientOptions Build()
		{
			var options = new FluentHttpClientOptions
			{
				Timeout = _timeout,
				BaseUrl = _baseUrl,
				Identifier = Identifier,
				Headers = _headers,
				Middleware = _middleware,
				RequestBuilderDefaults = _requestBuilderDefaults,
				HttpMessageHandler = _httpMessageHandler
			};
			return options;
		}

		/// <summary>
		/// Register to <see cref="FluentHttpClientFactory"/>, same as <see cref="FluentHttpClientFactory.Add"/> for convience.
		/// </summary>
		public FluentHttpClientBuilder Register()
		{
			_fluentHttpClientFactory.Add(this);
			return this;
		}
	}
}