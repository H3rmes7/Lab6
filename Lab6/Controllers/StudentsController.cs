using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab6.Data;
using Lab6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : Controller
    {

        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get collection of Students.
        /// </summary>
        /// <returns>A collection of Students</returns>
        /// <response code="200">Returns a collection of Students</response>
        /// <response code="500">Internal error</response>      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Student>>> Get()
        {
            return Ok(await _context.Students.ToListAsync());
        }

        [HttpGet("{ID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> GetById(Guid ID)
        {
            return Ok(await _context.Students.FindAsync(ID));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> CreateAsync([Bind("FirstName,LastName,Program")] StudentBase studentBase)
        {
            Student student = new Student
            {
                FirstName = studentBase.FirstName,
                LastName = studentBase.LastName,
                Program = studentBase.Program
            };

            _context.Add(student);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = student.ID }, student);
        }

        [HttpPut("{ID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> Update(Guid ID, [Bind("FirstName,LastName,Program")] StudentBase studentBase)
        {
            Student student = new Student
            {
                FirstName = studentBase.FirstName,
                LastName = studentBase.LastName,
                Program = studentBase.Program
            };

            if (!StudentExists(ID))
            {
                return NotFound("Student not found");
            }

            Student dbStudent = await _context.Students.FindAsync(ID);
            dbStudent.FirstName = student.FirstName;
            dbStudent.LastName = student.LastName;
            dbStudent.Program = student.Program;


            _context.Update(dbStudent);
            await _context.SaveChangesAsync();

            return Ok(dbStudent);
        }

        [HttpDelete("{ID}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteConfirmed(Guid ID)
        {
            var student = await _context.Students.FindAsync(ID);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Accepted();
        }

        private bool StudentExists(Guid ID)
        {
            return _context.Students.Any(e => e.ID == ID);
        }
    }
}