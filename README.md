# TodoWebApp
A todo web application

To setup your database, do the following

specify your database server instance in appsettings.json (do this for the API and Web.Client project)

How to setup your database for the API project
 
select the API project as the startup project
open the package manager console

select "Infrastructure/Persistence" as the default project
run the following comamnds
add-migration your_migration_name -context ApplicationDbContext this will run migrations
update-database -context ApplicationDbContext


How to setup your database for the Web Client project
 
select the Web Client project as the startup project
open the package manager console

select "Presentation/Web.Client" as the default project
run the following comamnds
add-migration your_migration_name -context AppDbContext this will run migrations
update-database -context AppDbContext

END POINTS

identity server base url: https://localhost:5001
Token endpoint: https://localhost:5001/connect/token
Todo API Url: https://localhost:5000/api/v1/todo
  
Metrics endpoint: https://localhost:5000/metrics-text   (Grafana dashboard to be integrated)
