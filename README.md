# Noteboard App

Welcome to the **Noteboard App**! This is a backend part of noteboard, for frontend view [NoteboardFrontend](https://github.com/Muhammad-Shah-zaib/NoteBoard), 

> Noteboard React-based web application designed for managing and organizing your notes and whiteboards. It includes features for creating, reading, updating, and deleting (CRUD) both notes and whiteboards, and it comes with a markdown editor for rich text note-taking and display.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete notes and whiteboards.
- **User Management**: Associates notes and whiteboards with users.
- **Email Verification**: Simple authentication based on email verification.
- **SMTP Integration**: Handles email requests for user verification.

## Getting started

### Prerequisites

- [.NET Core SDK 8.0.2](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

 1. clone the repo
``` bash
  git clone https://github.com/Muhammad-Shah-zaib/NoteBoardServer
```
2. navigate to folder
``` bash
  cd NoteBoardServer
```
3. install dependencies
``` bash
  dotnet restore
```
4. run the server
``` bash
  dotnet run
```


### Configuration

append smtpsettings in appsettings.json

``` js
"SmtpSettings": {
    "Server": "your_smpty_server",
    "Port": port_number,
    "SenderName": "your_name",
    "SenderEmail": "your_email",
    "Username": "your_email",
    "Password": "your_email_password"
  }
```
