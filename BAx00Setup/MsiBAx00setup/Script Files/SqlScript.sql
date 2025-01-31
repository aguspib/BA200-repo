--------------------------------------------------------------------------------------------------
--                                                                            
--  File Name:   sqlscript.sql                                   
--                                                                            
--  Description: Blank SQL script                                          
--                                                                            
--  Comments:    This blank script is intended for advanced users. 
--               To create a script from an existing database with step-by-step  
--		         instructions, use the Database Import Wizard. 
--                                                                                                               
---------------------------------------------------------------------------------------------------

USE master;
GO
ALTER DATABASE [Ax00] 
SET SINGLE_USER 
WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE [Ax00];
