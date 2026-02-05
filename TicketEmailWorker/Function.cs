//using Amazon.Lambda.Core;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System.Threading;
//using TicketEmailWorker.Contracts;
//using TicketEmailWorker.EFDbContext;
//using TicketEmailWorker.Email;
//using TicketEmailWorker.TicketDetails;

//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

//namespace TicketEmailWorker;

//public class Function
//{
//    private static ServiceProvider? _serviceProvider;

//    public async Task FunctionHandler(TicketEmailRequest request, ILambdaContext context)
//    {
//        if (request == null || request.TicketDetailId <= 0)
//        {
//            context.Logger.LogLine("❌ Invalid request received");
//            return;
//        }

//        context.Logger.LogLine($"✅ TicketEmailWorker started for TicketDetailId = {request.TicketDetailId}");

//        // Call Flight API endpoint to get ticket details using refit library

//        await SendCancelNotificationToAdmin(ticketDetails).ConfigureAwait(false);

//        // Pass the existing command instance to the handler
//        await SendCancellationNotificationToUser(ticketDetails).ConfigureAwait(false);

//        // Step 1 Call Refit and get ticket details
//        // Step 2 Send email to admin and user

//    }

//    private async Task SendCancelNotificationToAdmin(TicketDetails ticketDetails)
//    {
//        // Implementation for sending cancellation notification to admin
//    }

//    private async Task SendCancellationNotificationToUser(TicketDetails ticketDetails)
//    {
//        // Implementation for sending cancellation notification to user
//    }


//}



using Amazon.Auth.AccessControlPolicy;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Mustache;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using TicketEmailWorker.Contracts;
using TicketEmailWorker.Email;
using TicketEmailWorker.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TicketEmailWorker
{
    public class Function
    {
        private static ServiceProvider? _serviceProvider;

        public async Task FunctionHandler(TicketEmailRequest request, ILambdaContext context)
        {
            if (request == null || request.TicketDetailId <= 0)
            {
                context.Logger.LogLine("❌ Invalid request received (TicketDetailId missing)");
                return;
            }

            context.Logger.LogLine($"✅ TicketEmailWorker started for TicketDetailId = {request.TicketDetailId}");

            _serviceProvider ??= BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var flightApi = scope.ServiceProvider.GetRequiredService<IFlightApiClient>();

            try
            {
                // ✅ Step 1: Call FlightAPI to get TicketDetails
                var ticketDetails = await flightApi.GetTicketDetails(request.TicketDetailId);

                if (ticketDetails == null)
                {
                    context.Logger.LogLine($"❌ TicketDetails not found for TicketDetailId={request.TicketDetailId}");
                    return;
                }

                context.Logger.LogLine($"✅ Ticket fetched: Username={ticketDetails.Username}, Email={ticketDetails.EmailId}");

                // ✅ Step 2: Send Admin Email
                await SendCancelNotificationToAdmin(ticketDetails, configuration, emailService, context, CancellationToken.None);

                // ✅ Step 3: Send User Email
                await SendCancellationNotificationToUser(ticketDetails, configuration, emailService, context, CancellationToken.None);

                context.Logger.LogLine("✅ All cancellation emails sent successfully!");
            }
            catch (ApiException apiEx)
            {
                context.Logger.LogLine($"❌ FlightAPI Error: {apiEx.StatusCode} | {apiEx.Message}");
                context.Logger.LogLine($"❌ Response Content: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"❌ Exception Occurred: {ex.Message}");
            }
        }

        // ✅ DI Setup
        //private static ServiceProvider BuildServiceProvider()
        //{
        //    var services = new ServiceCollection();

        //    // ✅ Load appsettings.json (local)
        //    // AWS Lambda me real config tum ENV Variables se dena
        //    var configuration = new ConfigurationBuilder()
        //    .SetBasePath(AppContext.BaseDirectory)
        //    .AddJsonFile("appsettings.json", optional: false)
        //    .AddEnvironmentVariables()
        //    .Build();



        //    services.AddSingleton<IConfiguration>(configuration);

        //    // ✅ Email Service
        //    services.AddScoped<IEmailService, EmailService>();

        //    // ✅ Refit Client
        //    // appsettings.json -> FlightAdmin:BaseURL use karna
        //    var baseUrl = configuration["FlightAdmin:BaseURL"];

        //    if (string.IsNullOrWhiteSpace(baseUrl))
        //        throw new Exception("❌ FlightAdmin:BaseURL missing in appsettings / ENV");

        //    services.AddRefitClient<IFlightApiClient>()
        //        .ConfigureHttpClient(c =>
        //        {
        //            c.BaseAddress = new Uri(baseUrl);
        //        });

        //    return services.BuildServiceProvider();
        //}


        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            var basePath = Directory.GetCurrentDirectory(); // ✅ This is the correct path for Lambda Test Tool

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddScoped<IEmailService, EmailService>();

            var baseUrl = configuration["FlightAdmin:BaseURL"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new Exception("❌ FlightAdmin:BaseURL missing in appsettings / ENV");

            services.AddRefitClient<IFlightApiClient>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(baseUrl);
                });

            return services.BuildServiceProvider();
        }





        // ✅ Admin Email
        private async Task SendCancelNotificationToAdmin(
            TicketDetails ticketDetails,
            IConfiguration configuration,
            IEmailService emailService,
            ILambdaContext context,
            CancellationToken cancellationToken)
        {
            var toEmail = configuration["FlightAdmin:EmailConfirmToEmail"] ?? "mohammed.moinashraf@gmail.com";
            var fromEmail = configuration["FlightAdmin:EmailConfirmFromEmail"] ?? "support@codetosolutions.com";
            var ccEmail = configuration["FlightAdmin:EmailConfirmCcEmail"] ?? "";

            context.Logger.LogLine($"✅ Admin ToEmail = {toEmail}");

            var emailBody = GetConfirmTicketNotificationBody(ticketDetails);

            var subject =
                $"User {ticketDetails.Username} has Cancelled his flight from {ticketDetails.FromCity} to {ticketDetails.ToCity} on {ticketDetails.CancelledAt}";

            context.Logger.LogLine("Email body preview (Admin): " + emailBody.Substring(0, Math.Min(200, emailBody.Length)));

            await emailService.SendEmail(fromEmail, toEmail, subject, ccEmail, emailBody, cancellationToken)
                .ConfigureAwait(false);

            context.Logger.LogLine("✅ Admin email sent");
        }

        // ✅ User Email
        private async Task SendCancellationNotificationToUser(
            TicketDetails ticketDetails,
            IConfiguration configuration,
            IEmailService emailService,
            ILambdaContext context,
            CancellationToken cancellationToken)
        {
            var toEmail = ticketDetails.EmailId;

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                context.Logger.LogLine("⚠️ User email not found, skipping user mail");
                return;
            }

            var fromEmail = configuration["FlightAdmin:EmailConfirmFromEmail"] ?? "support@codetosolutions.com";
            var ccEmail = configuration["FlightAdmin:EmailConfirmCcEmail"] ?? "";

            context.Logger.LogLine($"✅ User ToEmail = {toEmail}");

            var emailBody = GetConfirmTicketNotificationBody(ticketDetails);

            var subject =
                $"As per your request, Your Ticket has been Cancelled for {ticketDetails.FromCity} to {ticketDetails.ToCity} and below is updated details";

            context.Logger.LogLine("Email body preview (User): " + emailBody.Substring(0, Math.Min(200, emailBody.Length)));

            await emailService.SendEmail(fromEmail, toEmail, subject, ccEmail, emailBody, cancellationToken)
                .ConfigureAwait(false);

            context.Logger.LogLine("✅ User email sent");
        }

        // ✅ Email Body (simple HTML)
        private string GetConfirmTicketNotificationBody(TicketDetails ticketDetails)
        {
            FormatCompiler compiler = new FormatCompiler();
            var template = Resource.ticket_confirmation_notify;
            Generator generator = compiler.Compile(template);
            generator.KeyNotFound += (obj, args) =>
            {
                args.Substitute = "";
                args.Handled = true;
            };

            var data = new Dictionary<string, object>
            {
                { "TicketDetails" , ticketDetails },
                { "FlightDate", ticketDetails.FormattedFlightDate },
                { "DepartureTime", ticketDetails.FormattedDepartureTime },
                { "ArrivalTime", ticketDetails.FormattedArrivalTime }
            };
            var result = generator.Render(data);

            return result;
        }
    }
}

