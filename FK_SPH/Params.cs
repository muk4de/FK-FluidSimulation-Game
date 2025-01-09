using System;
using FK_CLI;

static class Params
{
    public static int FlowedParticleNum = 0;

    // ���q�ǉ��\�Ȕ͈�
    public static Rectangle ClickableRect;

    // ���̗��q�̃}�e���A��
    public static fk_Material ParticleMaterial = new fk_Material();

    // �Ǘ��q�̃}�e���A��
    public static fk_Material WallMaterial = new fk_Material();

    // ��������
    public static Rectangle ParticleRect;

    // ��ԕ����̈�
    public static Rectangle CellRect;

    // �ǋ��E
    public static Rectangle[] Rects;

    // �����ǂ̃I�t�Z�b�g
    public static fk_Vector MoveWallOffset = new fk_Vector();

    // ������
    public static Rectangle MoveWallRect;

    // �������̃I�t�Z�b�g
    public static fk_Vector MoveFloorOffset = new fk_Vector();

    // ������
    public static Rectangle MoveFloorRect;

    // ��]����ǂ̃I�t�Z�b�g
    public static fk_Vector RotateOffset = new fk_Vector();

    // ��]�����
    public static Rectangle[] RotateRect_1;

    public static Rectangle[] RotateRect_2;

    public static Rectangle[] RotateRect_3;

    // �\������T�C�Y(1�ȉ�)
    public static double ShapeRad;

    // ���q�̉e���͈�
    public static double SmoothRad;

    // ���q�̏d��
    public static double Mass;

    // ���͂̍���
    public static double Stiffness;

    // ����x
    public static double Density0;

    // �S���W��
    public static double Viscosity;

    // �d��
    public static fk_Vector Gravity;

    // ���ԍ���
    public static double Dt;
}