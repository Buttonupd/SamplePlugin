using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
namespace SamplePlugin
{
    // Create a public class that is going to reference
    // the Iplugin package for interacting
    // with the Execute function as required by the plug-in
    public class FollowUpPlugin : IPlugin
    {

        // Reference The Execute Function 
        public void Execute(IServiceProvider serviceProvider)
        {

            // Provide context for the trace and logs mechanisms 
            // Capture output 
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Iplugin Connect to Registers
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));


            // Capture conditions whic evaluates according to code
            // Read manual
            if (context.InputParameters.Contains("Target") && 
                context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                // develop a reference to the IOrganization function
                // and add it as an internal call to create system feautures .
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Input and call the previous function
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


                // Push a try catch statement and order the priority of different conditions 

                try
                {
                    Entity followup = new Entity("task");
                    followup["subject"] = "Send e-mail to the new customer";
                    followup["description"] = "Follow up with the customer. Check if there are any new issues";
                    followup["scheduledstart"] = DateTime.Now.AddDays(7);
                    followup["scheduledend"] = DateTime.Now.AddDays(7);
                    followup["category"] = context.PrimaryEntityName;

                    // Provide substatnce for each evaluation
                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "account";

                        followup["regardingobjectid"] =
                        new EntityReference(regardingobjectidType, regardingobjectid);
                    }

                    // Create the task in Microsoft Dynamics CRM.
                    tracingService.Trace("FollowupPlugin: Creating the task activity.");
                    service.Create(followup);

                }

                catch (FaultException<OrganizationServiceFault> ex) 
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch(Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
                {

                }
                {

                }
            }

        }
    }
}
