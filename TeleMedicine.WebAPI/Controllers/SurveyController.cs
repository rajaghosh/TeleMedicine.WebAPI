using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Linq;
using Telemedicine.Service.Models;

namespace TeleMedicine.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {


        [HttpGet]
        public async Task<List<SurveyStep>> getQuestions()
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            //Response.ContentType = "application/json";
            return await survey.getQuestions();
        }

        [HttpPost]
        public async Task<int> updateAnswer(System.Text.Json.JsonElement jObject)
        {
            int submissionId;
            if (jObject.TryGetProperty("submissionID", out JsonElement submissionIdElement)){
                submissionId = submissionIdElement.GetInt32();
            }
            else
            {
                submissionId = 0;
            }

            

            System.Text.Json.JsonElement.ObjectEnumerator jsonElements = jObject.GetProperty("answers").EnumerateObject();
            List<SurveyAnswer> surveyAnswers = new List<SurveyAnswer>();
            while (jsonElements.MoveNext())
            {
                if (jsonElements.Current.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    string answer = string.Empty;
                    System.Text.Json.JsonElement.ArrayEnumerator items = jsonElements.Current.Value.EnumerateArray();
                    while (items.MoveNext())
                    {
                        if (answer.Length > 0)
                            answer += ",";
                        answer += items.Current.GetString();
                    }
                    surveyAnswers.Add(new SurveyAnswer { SubmissionID = submissionId, QuestionID = int.Parse(jsonElements.Current.Name), Answer = answer });

                }
                else
                {
                    surveyAnswers.Add(new SurveyAnswer { SubmissionID = submissionId, QuestionID = int.Parse(jsonElements.Current.Name), Answer = jsonElements.Current.Value.ToString() });
                }
            }

            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();

            return await survey.updateAnswer(surveyAnswers);
        }

        [HttpPost]
        public async Task<PaymentResult> makePayment(PaymentRequest paymentRequest)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.MakePayment(paymentRequest);
        }

        [HttpPost]
        public async Task<IFrameKey> getIframeKey(Origin origin)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.GetIframeKey(origin.origin);
        }

        [HttpPost]
        public async Task<Config> GetConfig()
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.getConfig();
        }

        [HttpPost]
        public async Task<List<SurveyRule>> GetRules()
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.getRules();
        }

        [HttpPost]
        public async Task<bool> SendOTP(PatiantInformation patient)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.SendOTP(patient);
            }

        [HttpPost]
        public async Task<PatiantInformation> GetPatientAnswers(PatiantInformation patient)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.GetPatientAnswers(patient);
        }

        [HttpPost]
        public async Task<PatiantInformation> UpdateSubmissionAfterPayment(PatiantInformation patient)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.UpdateSubmissionAfterPayment(patient);
        }

        [HttpPost]
        public async Task<int> InsertRefill(PatiantInformation patient)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.InsertRefill(patient);
        }

        [HttpPost]
        public async Task UpdateRefillAsPaid(PatiantInformation patient)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            await survey.UpdateRefillAsPaid(patient);
        }

        [HttpPost]
        public async Task<Rate> GetRate(Rate rate)
        {
            Telemedicine.Service.Managers.Survey survey = new Telemedicine.Service.Managers.Survey();
            return await survey.GetRate(rate.RateName);
        }

    }

}
