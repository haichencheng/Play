package frc.robot.commands;

import frc.robot.commands.*;
import frc.robot.subsystems.*;
import static frc.robot.Constants.*;

import java.util.function.DoubleSupplier;
import java.util.List;

import edu.wpi.first.wpilibj.controller.*;
import edu.wpi.first.wpilibj.geometry.*;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import edu.wpi.first.wpilibj.trajectory.Trajectory;
import edu.wpi.first.wpilibj.trajectory.TrajectoryConfig;
import edu.wpi.first.wpilibj.trajectory.TrajectoryGenerator;
import edu.wpi.first.wpilibj.kinematics.ChassisSpeeds;

import edu.wpi.first.wpilibj2.command.Command;
import edu.wpi.first.wpilibj2.command.SwerveControllerCommand;


public class DriveCommandFactory {

  private static DrivetrainSubsystem s_drivetrainSubsystem = DrivetrainSubsystem.getInstance();

  /**
   * Use this to pass the autonomous command to the main {@link Robot} class.
   *
   * @return the command to run in autonomous
   */
  public static Command getAutonomousCommand() {

    var command =
      new TrajectoryDriveCommand(
          List.of(
            new Translation2d(1, 0), 
            new Translation2d(2, 1)
          ),
          new Pose2d(3, 1, new Rotation2d(0)),
          false);
    return command;
  }

  // use field relative coordinate, move to target.
  public static Command getDriveToCommand(double p_targetX, double p_targetY, double p_targetAngleRad)
  {
    return new TrajectoryDriveCommand(List.of(), new Pose2d(p_targetX, p_targetY, new Rotation2d(p_targetAngleRad)), true); 
  }
}