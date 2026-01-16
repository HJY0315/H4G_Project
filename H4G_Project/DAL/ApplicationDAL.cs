using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using H4G_Project.Controllers;
using System.Collections;
using System.ComponentModel;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using H4G_Project.Models;
using H4G_Project.Services;

namespace H4G_Project.DAL
{
    public class ApplicationDAL
    {
        FirestoreDb db;

        public ApplicationDAL()
        {
            string jsonPath = "./DAL/config/squad-60b0b-firebase-adminsdk-fbsvc-582ee8d43f.json";
            string projectId = "squad-60b0b";
            using StreamReader r = new StreamReader(jsonPath);
            string json = r.ReadToEnd();


            db = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                JsonCredentials = json
            }.Build();
        }

        public async Task<bool> AddApplication(Application application, IFormFile medicalReport)
        {
            try
            {
                var fileService = new FileService();
                string medicalReportUrl = await fileService.SaveFileLocally(medicalReport);

                DocumentReference docRef = db.Collection("applicationForms").Document();
                Dictionary<string, object> NewApplication = new Dictionary<string, object>
                {
                    {"CaregiverName", application.CaregiverName},
                    {"ContactNumber", application.ContactNumber},
                    {"DisabilityType", application.DisabilityType},
                    {"Email", application.Email},
                    {"FamilyMemberName", application.FamilyMemberName},
                    {"Notes", application.Notes},
                    {"Occupation", application.Occupation},
                    {"MedicalReportUrl", medicalReportUrl} // store local URL in Firestore
                };

                await docRef.SetAsync(NewApplication);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding application: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Application>> GetAllApplications()
        {
            List<Application> applicationList = new List<Application>();

            Query allApplicationsQuery = db.Collection("applicationForms");
            QuerySnapshot snapshot = await allApplicationsQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    Application data = document.ConvertTo<Application>();
                    applicationList.Add(data);
                }
            }

            return applicationList;
        }

    }
}