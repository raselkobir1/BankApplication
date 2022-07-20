﻿using Bank.Entity.MongoModels;
using Bank.Service.MongoServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApplication.Web.Controllers.Mongo
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly CourseService _courseService;
        public StudentsController(StudentService studentService, CourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAll()
        {
            var students = await _studentService.GetStudentsAsync();
            return Ok(students);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetById(string id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            if(student.Courses.Count > 0)
            {
                var tempList = new List<Course>();
                foreach (var courseId in student.Courses)
                {
                    var course = await _courseService.GetByIdAsync(courseId);
                    if(course != null)
                    {
                        tempList.Add(course);   
                    }
                }
                student.CourseList = tempList;
            }
            return Ok(student);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            await _studentService.CreateAsync(student);
            return Ok(student);
        }
        [HttpPut]
        public async Task<IActionResult> Update(string id, Student updatedStudent)
        {
            var queriedStudent = await _studentService.GetStudentByIdAsync(id);
            if (queriedStudent == null)
            {
                return NotFound();
            }
            await _studentService.UpdateAsync(id, updatedStudent);
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            await _studentService.DeleteAsync(id);
            return NoContent();
        }
    }
}