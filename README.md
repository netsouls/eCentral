eCentral
=============

eCentral - ASP.NET 3.0 MVC Architecture

Solution Architecture 
---------------------

- eCentral.Core - Core functionality, Inversion of Control(Ioc),  and domain objects
- eCentral.Data - Creates the map between the domain object and database tables, if the database does not exists create the database tables
- eCentral.Services - Interface oriented service implementation
- eCentral.Web.Framework - Web application base framework - the functionality is inherited by the Web applications. The framework also sets up the Ioc for all the interfaces and the services. 
- eCentral.Web - Web application

Software Dependencies 
---------------------
- Visual Studio 2010
- Internet Information Service (IIS) Manager - The application will be run on [http://localhost:8081](http://localhost:8081/) on local development environment
- Microsoft SQL Server Express 2008 and Management Tools [Download](http://www.microsoft.com/en-us/download/details.aspx?id=23650)
- NuGet Package Manager Extension - automate the process of downloading and installing third party packages [Download](http://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c/)
