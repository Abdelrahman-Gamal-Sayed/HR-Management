using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Temp.Models;
using Temp.ViewModels;

namespace Temp.Controllers
{
    public class HomeController :Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
                              IWebHostEnvironment hostingEnvironment) {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public ViewResult Index() {
            // retrieve all the employees
            var model = _employeeRepository.GetAllEmployee();
            // Pass the list of employees to the view
            return View(model);
        }
        [HttpGet]
        public ViewResult Edit(int id) {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model) {
            if(ModelState.IsValid) {

                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if(model.Photo != null) {
                    if(model.ExistingPhotoPath != null) {
                     string filePath=   Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUplodedFile(model);
                }
             

              
                _employeeRepository.Update(employee);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        private string ProcessUplodedFile(EmployeeCreateViewModel model) {
            string uniqueFileName = null;

            // If the Photo property on the incoming model object is not null, then the user
            // has selected an image to upload.
            if(model.Photo != null) {
                // The image must be uploaded to the images folder in wwwroot
                // To get the path of the wwwroot folder we are using the inject
                // HostingEnvironment service provided by ASP.NET Core
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                // To make sure the file name is unique we are appending a new
                // GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                using(var fileStream = new FileStream(filePath, FileMode.Create)) {
                    model.Photo.CopyTo(fileStream);
                }
                
            }

            return uniqueFileName;
        }

        [HttpGet]
        public ViewResult Create() {
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model) {
            if(ModelState.IsValid) {
                string uniqueFileName = ProcessUplodedFile(model);

                Employee newEmployee = new Employee {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }
        public ViewResult Details(int? id) {


            Employee employee = _employeeRepository.GetEmployee(id.Value);

            if(employee==null) {

                Response.StatusCode = 404;
                return View("EmployeeNotFound",id);
            }
              return View(new HomeDetailsViewModel() { Employee = employee, PageTitle= "Employee Details" });
        }

    }
}
