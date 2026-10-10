using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bidding_API.Helpers
{
    public static class SafeExecutionHelper
    {
        /// <summary>
        /// UAT-07 Final Profile Polish — was previously a throwaway
        /// <c>Microsoft.Extensions.Logging</c> instance created via
        /// <c>LoggerFactory.Create(builder =&gt; builder.AddConsole())</c>, which only ever wrote to
        /// the process's own console — never to <c>apilogs.txt</c> or its archives, since it was
        /// completely disconnected from the app's real registered logging pipeline. Every exception
        /// this class ever caught (including the generic catch-all below) was silently lost the
        /// moment the console output wasn't being watched live. Switched to the exact same
        /// <c>NLog.LogManager.GetCurrentClassLogger()</c> pattern already used by
        /// <see cref="Bidding_API.ExceptionHandler.LoggerManager"/> (the app's real logger, which
        /// does write to the file-based log) — same NLog pipeline, same config, same output file.
        /// </summary>
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public interface IHasErrorCode
        {
            string ErrorCode { get; }
        }

        public class NotFoundException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public NotFoundException(string message) : this("NotFound", message) { }
            public NotFoundException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class ConflictException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public ConflictException(string message) : this("Conflict", message) { }
            public ConflictException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class BadRequestException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public BadRequestException(string message) : this("BadRequest", message) { }
            public BadRequestException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class InternalServerErrorException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public InternalServerErrorException(string message) : this("InternalServerError", message) { }
            public InternalServerErrorException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class UnauthorizedException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public UnauthorizedException(string message) : this("Unauthorized", message) { }
            public UnauthorizedException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class ForbiddenException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public ForbiddenException(string message) : this("Forbidden", message) { }
            public ForbiddenException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class MethodNotAllowedException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public MethodNotAllowedException(string message) : this("MethodNotAllowed", message) { }
            public MethodNotAllowedException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class NotAcceptableException : Exception, IHasErrorCode
        {
            public string ErrorCode { get; }
            public NotAcceptableException(string message) : this("NotAcceptable", message) { }
            public NotAcceptableException(string errorCode, string message) : base(message)
            {
                ErrorCode = errorCode;
            }
        }

        public static async Task<IActionResult> ExecuteSafelyAsync(
            Func<Task<IActionResult>> action,
            Func<string, string, int, IActionResult> createErrorResponse)
        {
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                return createErrorResponse("RequestCanceled", "The request was canceled by the client.", 499);
            }
            catch (BadRequestException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "BadRequest", ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (InternalServerErrorException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(
                    ex.ErrorCode ?? "InternalServerError",
                    ex.Message,
                    StatusCodes.Status500InternalServerError);
            }
            catch (UnauthorizedException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "Unauthorized", ex.Message, StatusCodes.Status401Unauthorized);
            }
            catch (ForbiddenException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "Forbidden", ex.Message, StatusCodes.Status403Forbidden);
            }
            catch (MethodNotAllowedException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "MethodNotAllowed", ex.Message, StatusCodes.Status405MethodNotAllowed);
            }
            catch (NotAcceptableException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "NotAcceptable", ex.Message, StatusCodes.Status406NotAcceptable);
            }
            catch (NotFoundException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "NotFound", ex.Message, StatusCodes.Status404NotFound);
            }
            catch (ConflictException ex)
            {
                _logger.Error(ex, $"Error: {ex.Message}");
                return createErrorResponse(ex.ErrorCode ?? "Conflict", ex.Message, StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                // UAT-07 Final Profile Polish — this is the generic catch-all that turns ANY
                // unrecognized exception into a 500 (e.g. the Profile-image endpoint's underlying
                // ImageRepository/Path/File calls, which throw plain framework exception types, not
                // one of the typed exceptions above). Logging the exception's own runtime type by
                // name here, since NLog's ${exception} layout renderer captures the message/stack
                // trace but a quick scannable type name in the message itself speeds up triage.
                _logger.Error(ex, $"Unhandled exception ({ex.GetType().FullName}): {ex.Message}");
                return createErrorResponse("InternalServerError", "An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
