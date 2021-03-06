package frc.robot.commands;
import static frc.robot.Constants.*;
import frc.robot.subsystems.DrivetrainSubsystem;

import edu.wpi.first.wpilibj2.command.CommandBase;
import edu.wpi.first.math.geometry.Rotation2d;
import edu.wpi.first.math.controller.HolonomicDriveController;
import edu.wpi.first.math.controller.PIDController;
import edu.wpi.first.math.controller.ProfiledPIDController;

import edu.wpi.first.math.trajectory.Trajectory;
import edu.wpi.first.math.kinematics.ChassisSpeeds;
import edu.wpi.first.wpilibj.Timer;


public class TrajectoryFollowCommand extends CommandBase {
  //@SuppressWarnings({"PMD.UnusedPrivateField", "PMD.SingularField"})
  private final DrivetrainSubsystem m_subsystem;

  private Trajectory m_trajectory;
  private Rotation2d m_endRotation;
  private final HolonomicDriveController m_controller;

  private final Timer m_timer = new Timer();

  public TrajectoryFollowCommand(
    Trajectory p_trajectory,
    boolean p_enablePID) 
  {
    m_subsystem = DrivetrainSubsystem.getInstance();
    // Use addRequirements() here to declare subsystem dependencies.
    addRequirements(m_subsystem);

    m_trajectory = p_trajectory;
    var endPose = p_trajectory.getStates().get(m_trajectory.getStates().size() - 1).poseMeters;
    this.withName("TrajectoryFollowTo_" + endPose.toString());
    m_endRotation = endPose.getRotation();

    var xController = new PIDController(AutoConstants.kPXController, 0, 0);
    var yController = new PIDController(AutoConstants.kPYController, 0, 0);     
    var thetaController = new ProfiledPIDController(AutoConstants.kPThetaController, 0, 0, AutoConstants.kThetaControllerConstraints);
    thetaController.enableContinuousInput(-Math.PI, Math.PI);
    m_controller = new HolonomicDriveController(xController, yController, thetaController);
    m_controller.setEnabled(p_enablePID);
  }

  // Called when the command is initially scheduled.
  @Override
  public void initialize() {
    m_timer.reset();
    m_timer.start();
  }

  @Override
  public void execute() {
    double curTime = m_timer.get();
    var desiredState = m_trajectory.sample(curTime);

    var targetChassisSpeeds = m_controller.calculate(m_subsystem.getPose(), desiredState, m_endRotation);
    m_subsystem.drive(targetChassisSpeeds);
  }

  // Called once the command ends or is interrupted.
  @Override
  public void end(boolean interrupted) {
    m_timer.stop();
    m_subsystem.drive(new ChassisSpeeds(0,0,0));
  }

  // Returns true when the command should end.
  @Override
  public boolean isFinished() {
    return m_timer.hasElapsed(m_trajectory.getTotalTimeSeconds());
  }
}
