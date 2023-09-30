# Architecture

The application will be implemented in Microsoft .NET v7, providing high performance and portability across platforms.

## Web site

The web site will be implemented as a hybrid SPA, with the following structure:

* **Home page**: Standard Razor page with search criteria and launch points for content. Searching for a phrase will navigate to the search application SPA.
* **Search application SPA**: Angular-powered page that will communicate woth the web-site API and provide translation services
* **Administration**: TBA 

## API

An API will be provided for the web site to use, along with other permitted clients.

## Database

Two databases will be leveraged, according to requirement.

### MS SQL

As part of the .NET stack, Microsoft SQL provides extensive opportunity and integration with .NET and will be 
used for tasks such as authentication, profile management, logging, etc.

### Vector DB _or_ optimised search index

A vector-database or an optimised search index will be leveraged for purposes of indexing content, particularly in order
to calculate word adjacency, potentially providing highly confident free-form translations.


