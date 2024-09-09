When deploying to production, remember to:

Add the key/value pairs in user-secrets as environment variables,

Set up a managed identity which has access to azure key vault and the database.

Add permissions to the identity via SQL queries to the database.
