# phase-2-back

For the app configuration I lowered the logging filter when in developing environement. More info is good. Also the
api key is stored ONLY in development configuration as it is a personal one with request limit. Production version 
will have a new enterprise key, but obviously not implemented here. So the api currently only work in development mode.

Dependency injections' main purpose is to make the code more maintainable and shorter. As the dependencies are not
instantiated in the each class but passed to where it's needed at runtime, making changes to the dependencies
are much easier and less likely to cause bugs. As we learned in lectures low coupling code is good. Also being able
to pass the depencies instead of hard coding them into each class is just less work. The configuration file mentioned
below was also injected into the controller. Another upside of this approach is that as dependencies are passed to 
the classes and not hard coded in, in unit testing we can easily pass mock dependencies for easier testing. Making the
whole process a lot quicker.
