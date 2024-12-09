# Sleep Preventer

### Prevent Screen Lock and Keep Teams Status Active (.NET WPF)

## Description
This .NET WPF project is designed to prevent Windows from locking the screen due to inactivity and to maintain the "Available" status in Microsoft Teams, even when the user is away from their desk. The application simulates user activity to ensure uninterrupted workflow and presence.

---

## Features
- **Prevent Screen Lock**: Stops the screen from locking or displaying the screensaver by mimicking user input.
- **Keep Teams Status Active**: Ensures that Microsoft Teams status remains "Available" regardless of actual user activity.
- **Customizable Intervals**: Allows users to set the frequency of activity simulation through a simple UI.
- **Lightweight**: Runs efficiently in the background with minimal system impact.
- **User-Friendly UI**: Intuitive interface to start/stop the functionality and configure settings.

---

## How It Works
1. **Simulated Input**: The application uses system calls to simulate small mouse movements or key presses.
2. **Windows API Integration**: Prevents the system from registering inactivity using native Windows APIs.
3. **Background Process**: Runs in the background and periodically generates activity signals based on user-defined intervals.

---

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/dimiano/sleep-preventer.git
2. Open the SleepPreventer.sln solution.
3. Build the solution to restore dependencies.
5. Run the application from IDE or the compiled executable.

---

## Requirements

1. Windows OS (I guess, it's 7 and later).
2. Framework: .NET 8.0.

---

## Contribution

Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request. Ensure your changes are well-documented and tested.

---

## License

This project is licensed under the [MIT License](https://opensource.org/license/mit).
