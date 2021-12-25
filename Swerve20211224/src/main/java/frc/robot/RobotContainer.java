// Copyright (c) FIRST and other WPILib contributors.
// Open Source Software; you can modify and/or share it under the terms of
// the WPILib BSD license file in the root directory of this project.

package frc.robot;

import edu.wpi.first.wpilibj.GenericHID;
import edu.wpi.first.wpilibj.XboxController;

import edu.wpi.first.wpilibj2.command.Command;
import edu.wpi.first.wpilibj2.command.CommandScheduler;
import edu.wpi.first.wpilibj2.command.button.Button;
import edu.wpi.first.wpilibj.geometry.Pose2d;
import edu.wpi.first.wpilibj.geometry.Rotation2d;
import edu.wpi.first.wpilibj.geometry.Translation2d;
import edu.wpi.first.wpilibj.shuffleboard.EventImportance;
import edu.wpi.first.wpilibj.shuffleboard.Shuffleboard;

import java.util.List;

import frc.robot.subsystems.DrivetrainSubsystem;
import frc.robot.commands.TrajectoryDriveCommand;
/**
 * This class is where the bulk of the robot should be declared. Since Command-based is a
 * "declarative" paradigm, very little robot logic should actually be handled in the {@link Robot}
 * periodic methods (other than the scheduler calls). Instead, the structure of the robot (including
 * subsystems, commands, and button mappings) should be declared here.
 */
public class RobotContainer {
  // The robot's subsystems and commands are defined here...
  private DrivetrainSubsystem m_drivetrainSubsystem;
  private final XboxController m_controller = new XboxController(0);

  /** The container for the robot. Contains subsystems, OI devices, and commands. */
  public RobotContainer() {

    m_drivetrainSubsystem = DrivetrainSubsystem.getInstance();

    // Set the scheduler to log Shuffleboard events for command initialize, interrupt, finish
    CommandScheduler.getInstance()
     .onCommandInitialize(
         command ->
             Shuffleboard.addEventMarker(
                 "Command initialized", command.getName(), EventImportance.kNormal));
    CommandScheduler.getInstance()
        .onCommandInterrupt(
            command ->
                Shuffleboard.addEventMarker(
                    "Command interrupted", command.getName(), EventImportance.kNormal));
    CommandScheduler.getInstance()
        .onCommandFinish(
            command ->
                Shuffleboard.addEventMarker(
                    "Command finished", command.getName(), EventImportance.kNormal));

    // Configure the button bindings
    configureButtonBindings();
  }

  /**
   * Use this method to define your button->command mappings. Buttons can be created by
   * instantiating a {@link GenericHID} or one of its subclasses ({@link
   * edu.wpi.first.wpilibj.Joystick} or {@link XboxController}), and then passing it to a {@link
   * edu.wpi.first.wpilibj2.command.button.JoystickButton}.
   */
  private void configureButtonBindings() {
    new Button(m_controller::getBackButton).whenPressed(m_drivetrainSubsystem::zeroGyroscope);
    // new Button(m_controller::getAButton).whenPressed(m_limelight::init);
    // new Button(m_controller::getXButton).whenPressed();
    // new Button(m_controller::getYButton).whenPressed(robot::robotInit);
  }

  /**
   * Use this to pass the autonomous command to the main {@link Robot} class.
   *
   * @return the command to run in autonomous
   */
  public Command getAutonomousCommand() {
    TrajectoryDriveCommand command =
      new TrajectoryDriveCommand(
          m_drivetrainSubsystem,
          // Pass through these two interior waypoints, making an 's' curve path
          List.of(new Translation2d(.5, .25), new Translation2d(1, -.25)),
          // End 1.5 meters straight ahead of where we started, facing forward
          new Pose2d(1.5, 0, new Rotation2d(0)));
    return command;
  }

  public Command getTranslationCommand(double xDistance, double yDistance)
  {
    TrajectoryDriveCommand command = new TrajectoryDriveCommand(
      m_drivetrainSubsystem,
      // Pass through these two interior waypoints, making an 's' curve path
      List.of(
        new Translation2d(xDistance/4, yDistance/4),
        new Translation2d(xDistance/2, yDistance/2), 
        new Translation2d(xDistance*3/4, yDistance*3/4)),
      new Pose2d(xDistance, yDistance, new Rotation2d(0)));
    return command;
  }

  public Command getTurnCommand(double rotateRad)
  {
    TrajectoryDriveCommand command = new TrajectoryDriveCommand(
      m_drivetrainSubsystem,
      // Pass through these two interior waypoints, making an 's' curve path
      List.of(),
      new Pose2d(0, 0, new Rotation2d(rotateRad)));
    return command;
  }

  public Command getMoveToCommand(double xDistance, double yDistance, double rotateRad)
  {
    TrajectoryDriveCommand command = new TrajectoryDriveCommand(
      m_drivetrainSubsystem,
      // Pass through these two interior waypoints, making an 's' curve path
      List.of(
        new Translation2d(xDistance/4, yDistance/4),
        new Translation2d(xDistance/2, yDistance/2), 
        new Translation2d(xDistance*3/4, yDistance*3/4)),
      new Pose2d(xDistance, yDistance, new Rotation2d(rotateRad)));
    return command;
  }

  public DrivetrainSubsystem getDriveTrainSubsystem()
  {
    return m_drivetrainSubsystem;
  }
}
