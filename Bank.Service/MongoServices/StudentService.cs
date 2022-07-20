using Bank.Entity.MongoModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.MongoServices
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> _student;
        public StudentService(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _student = database.GetCollection<Student>(settings.StudentsCollectionName);
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            try
            {
                return await _student.Find(s => true).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            
        }
        public async Task<Student> GetStudentByIdAsync(string id)
        {
            return await _student.Find<Student>(s => s.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Student> CreateAsync(Student student)
        {
            await _student.InsertOneAsync(student);
            return student;
        }
        public async Task UpdateAsync(string id, Student student)
        {
            await _student.ReplaceOneAsync(s=> s.Id == id, student);    
        }
        public async Task DeleteAsync(string id)
        {
            await _student.DeleteOneAsync(s=> s.Id == id);  
        }
    }
}
