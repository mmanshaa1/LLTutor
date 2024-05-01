using App.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Repository.Data.Contexts
{
    public class MainContextSeed
    {
        public static async Task SeedAsync(MainContext mainContext)
        {
            // seed course data
            if (!await mainContext.Courses.AnyAsync())
            {
                await mainContext.Courses.AddAsync(new Course()
                {
                    ModelId = 1,
                    Name = "Networks 1"
                });

                await mainContext.Courses.AddAsync(new Course()
                {
                    ModelId = 2,
                    Name = "Software Engineering"
                });

                await mainContext.Courses.AddAsync(new Course()
                {
                    ModelId = 3,
                    Name = "Computer Vision"
                });

                await mainContext.Courses.AddAsync(new Course()
                {
                    ModelId = 4,
                    Name = "Databases"
                });

                await mainContext.Courses.AddAsync(new Course()
                {
                    ModelId = 5,
                    Name = "Data Structures"
                });

                await mainContext.SaveChangesAsync();
            }
        }
    }
}
