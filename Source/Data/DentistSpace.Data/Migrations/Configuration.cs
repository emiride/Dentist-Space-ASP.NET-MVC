namespace DentistSpace.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    public sealed class Configuration : DbMigrationsConfiguration<DentistSpace.Data.DentistSpaceDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DentistSpace.Data.DentistSpaceDbContext context)
        {
            this.SeedRoles(context);

            var admin = this.SeedAdmin(context);
            var dentist = this.SeedDentists(context);
            var patient = this.SeedPatient(context, dentist);
            this.SeedPosts(context, dentist);

            context.SaveChanges();
        }

        private void SeedRoles(DentistSpaceDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var adminRole = new IdentityRole { Name = "Admin" };
                roleManager.Create(adminRole);

                var dentistRole = new IdentityRole { Name = "Dentist" };

                roleManager.Create(dentistRole);

                var patientRole = new IdentityRole { Name = "Patient" };
                roleManager.Create(patientRole);
                context.SaveChanges();
            }
        }

        private Admin SeedAdmin(DentistSpaceDbContext context)
        {
            if (!context.Users.Any())
            {
                var userManager = new UserManager<User>(new UserStore<User>(context));
                var admin = new User()
                {
                    Email = "goran@yahoo.com.com",
                    UserName = "gorancvetkov",
                    FirstName = "Goran",
                    LastName = "Cvetkov",
                };

                userManager.Create(admin, "123456");
                userManager.AddToRole(admin.Id, "Admin");

                var userAdmin = new Admin()
                {
                    User = admin,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false
                };

                context.Admins.Add(userAdmin);
                context.SaveChanges();
                return userAdmin;
            }

            return null;
        }

        private Dentist SeedDentists(DentistSpaceDbContext context)
        {
            if (!(context.Users.Count() > 1))
            {
                var userManager = new UserManager<User>(new UserStore<User>(context));
                var dentist = new User()
                {
                    Email = "stamat@yahoo.com",
                    UserName = "stamat",
                    FirstName = "Stamat",
                    LastName = "Stamatov"
                };

                userManager.Create(dentist, "123456");
                userManager.AddToRole(dentist.Id, "Dentist");

                var userDentist = new Dentist()
                {
                    User = dentist,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                    IsAccepted = true
                };

                context.Dentists.Add(userDentist);
                context.SaveChanges();
                return userDentist;
            }

            return null;
        }

        private Patient SeedPatient(DentistSpaceDbContext context, Dentist dentist)
        {
            if (!(context.Users.Count() > 1))
            {
                var userManager = new UserManager<User>(new UserStore<User>(context));
                var patient = new User()
                {
                    Email = "aleksandra@yahoo.com",
                    UserName = "aleksandra",
                    FirstName = "Aleksandra",
                    LastName = "Aleksandrova"
                };

                userManager.Create(patient, "123456");
                userManager.AddToRole(patient.Id, "Patient");

                var userPatient = new Patient()
                {
                    User = patient,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                    IsAccepted = true
                };

                userPatient.Dentists.Add(dentist);
                context.Patients.Add(userPatient);
                context.SaveChanges();
                return userPatient;
            }

            return null;
        }

        private void SeedPosts(DentistSpaceDbContext context, Dentist dentist)
        {
            if (context.Posts.Count() == 0)
            {
                for (var i = 0; i < 30; i++)
                {
                    context.Posts.Add(new Post() { Content = "Test content" + i, CreatedOn = DateTime.Now, Dentist = dentist, IsDeleted = false, IsPublic = true });
                }

                context.SaveChanges();
            }
        }
    }
}
