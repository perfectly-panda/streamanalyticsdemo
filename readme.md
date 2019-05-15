/**********************************
*                                 *
*     Deployment Instructions     *
*                                 *
**********************************/

Azure Services:
1) App Service (website)
2) Azure Functions (You can use either web app service or consumption plan. I recomend consumption plan)
3) Azure SQL
4) Event Hub
5) Stream Analytics
6) Service Bus

Deployment:
1) If you want to adjust any of the settings, you can change the values in SQL Database > Script.PopulateSettings.sql
2) Deploy Azure SQL
3) Create a Service Bus with the following topics:
	MachineTopic
	OrderTopic
	AnalyticsTopic
	LogTopic
4) Create Event Hub with a secondary receiver.
5) Create a subscription for each topic for the website (remember the name- you will add it to the settings).
6) Deploy WebApplication to your website.
	6a) Update SQL Connection string.
	6b) Update Service Bus connection string and subscription name.
	6c) Update Event Hub connection string and connection for secondary receiver.
7) Deploy Event Generator functions project.
	7a) Update SQL Connection string.
	7b) Update Event Hub connection string.
8) Create Stream Analytics account and connect it to the Event Hub and Service Bus Analytics Topic.
	8a) Deploy SA job from Stream Analytics project.