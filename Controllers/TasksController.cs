using Consume.Helper;
using Consume.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Consume.Controllers
{
    public class TasksController : Controller
    {
        MyProjectAPI _api = new MyProjectAPI();
        // GET: Tasks
        public async Task<ActionResult> Tasks(Guid Id)
        {
            List<TasksData> Tasks = new List<TasksData>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/projects/{Id}/tasks");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                Tasks = JsonConvert.DeserializeObject<List<TasksData>>(results);
            }

            return View(Tasks);
        }

        [HttpGet]
        public async Task<ActionResult> TaskDetails(TasksData ProjectTSId, TasksData id)
        {
            var task = new TasksData();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/projects/{ProjectTSId}/tasks/{id}");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                task = JsonConvert.DeserializeObject<TasksData>(results);
            }

            return View(task);

        }
        public ActionResult CreateTask()
        {
            return View();

        }

        //[HttpPost]
        //public ActionResult CreateTask(TasksData ProjectTSId)
        //{
        //    HttpClient client = _api.Initial();
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    var response = client.PostAsJsonAsync($"api/projects/{ProjectTSId}/tasks/", ProjectTSId).Result;

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Tasks");
        //    }
        //    return View();
        //}

        [HttpPost]
        public ActionResult CreateTask(TasksData Project)
        {
            HttpClient client = _api.Initial();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.PostAsJsonAsync($"api/projects/{Project}/tasks/", Project).Result;
            if (response.IsSuccessStatusCode)
            {
                var newCountryTask = response.Content.ReadAsAsync<TasksData>();
                newCountryTask.Wait();

                var newCountry = newCountryTask.Result;
                TempData["SuccessMessage"] = $"Country {newCountry.Name} was successfully created.";

                return RedirectToAction("GetCountryById", new { countryId = newCountry.Id });
            }

            if ((int)response.StatusCode == 422)
            {
                ModelState.AddModelError("", "Country Already Exists!");
            }
            else
            {
                ModelState.AddModelError("", "Some kind of error. Country not created!");
            }
            return View();
        }
    }
}