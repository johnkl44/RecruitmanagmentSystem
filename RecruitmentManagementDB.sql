CREATE DATABASE RecruitManagementDB;

CREATE TABLE tblUsers (
    UserId INT PRIMARY KEY IDENTITY(1,1), 
    FirstName NVARCHAR(50) NOT NULL,      
    LastName NVARCHAR(50) NOT NULL,       
    DateOfBirth DATE NOT NULL,            
    Gender NVARCHAR(10) NOT NULL,        
    PhoneNumber NVARCHAR(15) NOT NULL,   
    Email NVARCHAR(100) NOT NULL UNIQUE, 
    Address NVARCHAR(200) NOT NULL,      
    State NVARCHAR(50) NOT NULL,          
    City NVARCHAR(50) NOT NULL,     
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL, 
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Candidate', 'Admin')),
    CreatedAt DATETIME DEFAULT GETDATE() NULL, 
    UpdatedAt DATETIME DEFAULT GETDATE() NULL
);
SELECT * FROM tblUsers

CREATE TABLE tblStates (
    StateId INT PRIMARY KEY IDENTITY,
    StateName NVARCHAR(50)
);
SELECT * FROM tblStates
 -- Get State
CREATE PROCEDURE SP_GetState
AS
BEGIN
    SELECT StateId, StateName FROM tblStates;
END

EXEC SP_GetState

CREATE TABLE tblCities (
    CityId INT PRIMARY KEY IDENTITY,
    CityName NVARCHAR(50),
    StateId INT FOREIGN KEY REFERENCES tblStates(StateId)
);

SELECT * FROM tblCities

CREATE PROCEDURE SP_CityDropDown
(
@StateId INT
)
AS
BEGIN
	SELECT CityId, CityName FROM tblCities WHERE StateId = @StateId
END

EXEC SP_CityDropDown 1

--- Read All Users

CREATE PROCEDURE SPR_Users
AS
BEGIN
	SELECT UserId,FirstName,LastName,DateOfBirth,Gender,PhoneNumber,Email,Address,State,City,Username,Password,Role FROM tblUsers WITH(NOLOCK)
END

EXEC SPR_Users

-- Get User by ID

ALTER PROCEDURE SP_GetUserByID
(
@UserId INT
)
AS
BEGIN
	SELECT UserId,FirstName,LastName,DateOfBirth,Gender,PhoneNumber,Email,Address,State,City,Username,Password,Role FROM tblUsers WITH(NOLOCK)
	WHERE UserId= @UserId
END 

EXEC SP_GetUserByID 5

-- Insert User

ALTER PROCEDURE SPI_User
(
	@FirstName NVARCHAR(50),      
    @LastName NVARCHAR(50),       
    @DateOfBirth DATE,            
    @Gender NVARCHAR(10),        
    @PhoneNumber NVARCHAR(15),   
    @Email NVARCHAR(100), 
    @Address NVARCHAR(200),      
    @State NVARCHAR(50),          
    @City NVARCHAR(50),     
    @Username NVARCHAR(50),
    @Password NVARCHAR(255), 
    @Role NVARCHAR(20)
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
		INSERT INTO tblUsers(FirstName,LastName,DateOfBirth,Gender,PhoneNumber,Email,Address,State,City,Username,Password,Role)
		VALUES(@FirstName,@LastName,@DateOfBirth,@Gender,@PhoneNumber,@Email,@Address,@State,@City,@Username,@Password,@Role)
		COMMIT TRAN
	END TRY
	BEGIN CATCH
			ROLLBACK TRAN
			    PRINT ERROR_MESSAGE()
	END CATCH
END 

SELECT * FROM tblUsers

--- Update User

ALTER PROCEDURE SPU_User
(
	@UserId INT,
	@FirstName NVARCHAR(50),      
    @LastName NVARCHAR(50),       
    @DateOfBirth DATE,            
    @Gender NVARCHAR(10),        
    @PhoneNumber NVARCHAR(15),   
    @Email NVARCHAR(100), 
    @Address NVARCHAR(200),      
    @State NVARCHAR(50),          
    @City NVARCHAR(50)
)
AS
BEGIN
DECLARE @RowCount INT = 0
	BEGIN TRY
		SET @RowCount=(SELECT COUNT(1) FROM tblUsers WITH (NOLOCK) WHERE UserId=@UserId)
		IF @RowCount > 0
			BEGIN
				BEGIN TRAN
					UPDATE tblUsers
					SET 
						FirstName = @FirstName,      
						LastName = @LastName,       
						DateOfBirth = @DateOfBirth,            
						Gender = @Gender,        
						PhoneNumber = @PhoneNumber,   
						Email = @Email, 
						Address = @Address,      
						State = @State,          
						City = @City   
					WHERE UserId =@UserId
				COMMIT TRAN
			END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END 

--- Delete User

CREATE PROCEDURE SPD_User
(
@UserId INT
)
AS
BEGIN
DECLARE @RowCount INT = 0
	BEGIN TRY
		SET @RowCount=(SELECT COUNT(1) FROM tblUsers WITH (NOLOCK) WHERE UserId=@UserId)
		IF @RowCount > 0
			BEGIN
				BEGIN TRAN
					DELETE FROM tblUsers
					WHERE UserId= @UserId
				COMMIT TRAN
			END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END 

--- Validate User

ALTER PROCEDURE SP_ValidateUser
(
    @UserName NVARCHAR(50),
    @Password NVARCHAR(255)
)
AS
BEGIN
    SELECT Role 
    FROM tblUsers
    WHERE Username = @UserName AND Password = @Password;
END;

--- change password user

CREATE PROCEDURE ChangePassword
(
@UserID INT,
@Password NVARCHAR(255)
)
AS
BEGIN
	UPDATE tblUsers 
	SET
	Password = @Password
	WHERE UserId= @UserID
END	

-- Get User By User name

ALTER PROCEDURE SP_GetUserByUsername
(    @UserName NVARCHAR(50))
AS
BEGIN
SELECT UserId,FirstName,LastName,DateOfBirth,Gender,PhoneNumber,Email,Address,State,City,Username,Password,Role FROM tblUsers WHERE Username = @Username
END

EXEC SP_GetUserByUsername 'Admin'

---===== tblJobPosting ====

--- Table JobPosting

CREATE TABLE tblJobPostings (
    JobId INT IDENTITY(1,1) PRIMARY KEY,
    JobTitle NVARCHAR(100) NOT NULL,
    JobDescription NVARCHAR(MAX) NOT NULL,
    RequiredSkills NVARCHAR(MAX),
    Experience NVARCHAR(50),
    SalaryRange NVARCHAR(50),
    Deadline DATE NOT NULL,
    JobStatus NVARCHAR(50) DEFAULT 'Active',
	PosterPhoto VARBINARY(MAX),
	PostingDate DATETIME DEFAULT GETDATE(),
    Author INT NOT NULL FOREIGN KEY REFERENCES tblUsers(UserId)
);

SELECT * FROM tblJobPostings

--- Insert JobPostings

ALTER PROCEDURE SP_JobCreation
(
	@JobTitle NVARCHAR(100),      
    @JobDescription NVARCHAR(MAX),       
    @RequiredSkills NVARCHAR(MAX),
    @Experience NVARCHAR(50),
    @SalaryRange NVARCHAR(50),
    @Deadline DATE,
    @JobStatus NVARCHAR(50),
    @Author INT,
	@PosterPhoto VARBINARY(MAX),
	@PostingDate Date 
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
		INSERT INTO tblJobPostings(JobTitle,JobDescription,RequiredSkills,Experience,SalaryRange,Deadline,JobStatus,Author,PosterPhoto,PostingDate)
		VALUES(@JobTitle,@JobDescription,@RequiredSkills,@Experience,@SalaryRange,@Deadline,@JobStatus,@Author,@PosterPhoto,@PostingDate)
		COMMIT TRAN
	END TRY
	BEGIN CATCH
			ROLLBACK TRAN
			    PRINT ERROR_MESSAGE()
	END CATCH
END 

--- Retrive all active JobPostings

ALTER PROCEDURE SPR_JobPostings
AS
BEGIN
	SELECT  JobId,JobTitle,JobDescription,RequiredSkills,Experience,SalaryRange,Deadline,JobStatus,Author,PosterPhoto,PostingDate FROM tblJobPostings WITH(NOLOCK)
	WHERE JobStatus = 'Active' ORDER BY Deadline;
END

EXEC SPR_JobPostings

--- UPDATE JObPostings

ALTER PROCEDURE SPU_Jobs
(
	@JobId INT,
	@JobTitle NVARCHAR(100),      
    @JobDescription NVARCHAR(MAX),       
    @RequiredSkills NVARCHAR(MAX),            
    @Experience  NVARCHAR(50),        
    @SalaryRange NVARCHAR(50),   
    @Deadline DATE, 
    @PosterPhoto VARBINARY(MAX)     
)
AS
BEGIN
DECLARE @RowCount INT = 0
	BEGIN TRY
		SET @RowCount=(SELECT COUNT(1) FROM tblJobPostings WITH (NOLOCK) WHERE JobId=@JobId)
		IF @RowCount > 0
			BEGIN
				BEGIN TRAN
					UPDATE tblJobPostings
					SET 
						JobTitle = @JobTitle,      
						JobDescription = @JobDescription,       
						RequiredSkills = @RequiredSkills,            
						Experience = @Experience ,        
						SalaryRange = @SalaryRange,   
						Deadline = @Deadline, 
						PosterPhoto = @PosterPhoto     
   
					WHERE JobId =@JobId
				COMMIT TRAN
			END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END 

--- Delete JobPostings
CREATE PROCEDURE SPD_JobPostings
(
@JobId INT
)
AS
BEGIN
DECLARE @RowCount INT = 0
	BEGIN TRY
		SET @RowCount=(SELECT COUNT(1) FROM tblJobPostings WITH (NOLOCK) WHERE JobId=@JobId)
		IF @RowCount > 0
			BEGIN
				BEGIN TRAN
					DELETE FROM tblJobPostings
					WHERE JobId=@JobId
				COMMIT TRAN
			END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END 

EXEC SPD_JobPostings 8

select * from tblJobPostings

--- Get Job by id

ALTER PROCEDURE SP_GetJobByID
(
@JobId INT
)
AS
BEGIN
	SELECT JobId,JobTitle,JobDescription,RequiredSkills,Experience,SalaryRange,Deadline,JobStatus,Author,PosterPhoto FROM tblJobPostings WITH(NOLOCK)
	WHERE JobId= @JobId
END 

EXEC SP_GetJobByID 3

---==== Table Application =====
---  Create tblApplications table

CREATE TABLE tblApplications (
    ApplicationID INT IDENTITY PRIMARY KEY,
    CandidateID INT FOREIGN KEY REFERENCES tblUsers(UserId),
    JobID INT FOREIGN KEY REFERENCES tblJobPostings(JobId),
    ResumeFile VARBINARY(MAX),
	ProfilePhoto VARBINARY(MAX),
    ApplicationStatus NVARCHAR(20) NOT NULL CHECK (ApplicationStatus IN ('Pending', 'Reviewed', 'Shortlisted', 'Rejected')),
    AppliedDate DATE
    
);


SELECT * FROM tblApplications

--- Insert into Application

ALTER PROCEDURE SPI_Application
(
    @CandidateID INT,
    @JobID INT ,
    @ResumeFile VARBINARY(MAX), 
	@ProfilePhoto VARBINARY(MAX),
    @ApplicationStatus NVARCHAR(20),
    @AppliedDate DATE
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
		INSERT INTO tblApplications(CandidateID,JobID,ResumeFile,ProfilePhoto,ApplicationStatus,AppliedDate)
		VALUES(@CandidateID,@JobID,@ResumeFile,@ProfilePhoto,@ApplicationStatus,@AppliedDate)
		COMMIT TRAN
	END TRY
	BEGIN CATCH
			ROLLBACK TRAN
			    PRINT ERROR_MESSAGE()
	END CATCH
END 

EXEC SPI_Application 10, 3, NULL, NULL, 'Pending', '2024-10-24';

--- Retriving submitted Applications admin

ALTER PROCEDURE SPR_AdminViewApplications
AS
BEGIN
    SELECT 
        app.ApplicationID,
        app.CandidateID,
        u.FirstName AS CandidateFirstName,
        u.LastName AS CandidateLastName,
        u.Email AS CandidateEmail,
        app.JobID,
        job.JobTitle,
        app.ApplicationStatus,
        app.AppliedDate,
        job.Author AS JobAuthor,
        job.PosterPhoto,
		app.ResumeFile,
		app.ProfilePhoto

    FROM 
        tblApplications app
    INNER JOIN 
        tblUsers u ON app.CandidateID = u.UserId
    INNER JOIN 
        tblJobPostings job ON app.JobID = job.JobId
    ORDER BY 
        app.AppliedDate DESC;
END;

EXEC SPR_AdminViewApplications

--===Updated Column From NVARCHAR TO VARBINARY===

ALTER TABLE tblApplications ALTER COLUMN ResumeFile VARBINARY(MAX);

ALTER TABLE tblApplications ADD ResumeFileBinary VARBINARY(MAX);

UPDATE tblApplications
SET ResumeFileBinary = CONVERT(VARBINARY(MAX), ProfilePhoto);

ALTER TABLE tblApplications DROP COLUMN ProfilePhoto;

EXEC sp_rename 'tblApplications.ResumeFileBinary', 'ProfilePhoto', 'COLUMN';

EXEC sp_help 'tblApplications';

ALTER TABLE tblApplications ADD ResumeFile VARBINARY(MAX);

--Candidate Applications View

CREATE PROCEDURE SPR_CandidateViewApplications
(
    @CandidateID INT
)
AS
BEGIN
    SELECT 
        app.ApplicationID,
        app.CandidateID,
        u.FirstName AS CandidateFirstName,
        u.LastName AS CandidateLastName,
        u.Email AS CandidateEmail,
        app.JobID,
        job.JobTitle,
        app.ApplicationStatus,
        app.AppliedDate,
        job.Author AS JobAuthor,
        job.PosterPhoto,
        app.ResumeFile,
        app.ProfilePhoto
    FROM 
        tblApplications app
    INNER JOIN 
        tblUsers u ON app.CandidateID = u.UserId
    INNER JOIN 
        tblJobPostings job ON app.JobID = job.JobId
    WHERE 
        app.CandidateID = @CandidateID
    ORDER BY 
        app.AppliedDate DESC;
END;

EXEC SPR_CandidateViewApplications 22

-- Update application status

ALTER PROCEDURE SPU_ApplicationStatus
    @ApplicationId INT,
    @ApplicationStatus NVARCHAR(20)
AS
BEGIN
    UPDATE tblApplications
    SET 
	ApplicationStatus = @ApplicationStatus
    WHERE 
	ApplicationId = @ApplicationId;
END;

--- Get Application bid

CREATE PROCEDURE SPR_GetApplicationById
(
    @ApplicationID INT
)
AS
BEGIN
    SELECT 
        app.ApplicationID,
        app.CandidateID,
        u.FirstName AS CandidateFirstName,
        u.LastName AS CandidateLastName,
        u.Email AS CandidateEmail,
        app.JobID,
        job.JobTitle,
        app.ApplicationStatus,
        app.AppliedDate,
        job.Author AS JobAuthor,
        job.PosterPhoto,
        app.ResumeFile,
        app.ProfilePhoto
    FROM 
        tblApplications app
    INNER JOIN 
        tblUsers u ON app.CandidateID = u.UserId
    INNER JOIN 
        tblJobPostings job ON app.JobID = job.JobId
    WHERE 
        app.ApplicationID = @ApplicationID
    ORDER BY 
        app.AppliedDate DESC;
END;

EXEC SPR_GetApplicationById 11

--ContactUs 

CREATE TABLE tblContactUS
(
ContactID INT PRIMARY KEY IDENTITY,
Name NVARCHAR(50),
EmailId NVARCHAR(50),
UserMessage NVARCHAR(500),
ResponseMessage NVARCHAR(500),
ResponseStatus NVARCHAR(25) 
);

SELECT * FROM tblContactUS

CREATE PROCEDURE SPI_ContactUs
    @Name NVARCHAR(50),
    @EmailId NVARCHAR(50),
    @UserMessage NVARCHAR(500)
AS
BEGIN
    INSERT INTO tblContactUS (Name, EmailId, UserMessage, ResponseMessage, ResponseStatus)
    VALUES (@Name, @EmailId, @UserMessage, NULL, 'Pending');
END;

