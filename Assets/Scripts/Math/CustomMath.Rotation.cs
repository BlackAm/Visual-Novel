using UnityEngine;

namespace BlackAm
{
    public static partial class CustomMath
    {
        /// <summary>
        /// 벡터 모드로 쿼터니언 구조체를 생성하여 리턴하는 메서드
        /// </summary>
        public static Quaternion GetQuaternion(float p_RearPart, Vector3 p_ImaginaryPart)
        {
            return new Quaternion(p_ImaginaryPart.x, p_ImaginaryPart.y, p_ImaginaryPart.z, p_RearPart);
        }
        
        /// <summary>
        /// 연산이 끝난 쿼터니언 모드의 x,y,z성분을 벡터로 리턴하는 메서드
        /// </summary>
        public static Vector3 TurnQuarternionVectorMode(this Quaternion p_TargetQuaternion)
        {
            return new Vector3(p_TargetQuaternion.x, p_TargetQuaternion.y, p_TargetQuaternion.z);
        }
        
        /// <summary>
        /// 특정한 벡터를 특정한 월드 벡터 방향으로 특정한 양만큼 회전시킨 노멀벡터를 리턴한다.
        ///
        /// 해당 벡터 변환의 결과물은 Transform.Rotate(pivot, degree)와 같다.
        /// 차이점은 해당 벡터는 파라미터로 '회전시킬 방향'을 삽입하고, Rotate함수는 '회전축'을 삽입한다는 차이점이 있다.
        ///
        /// 다만, Direction 벡터를 기준으로 회전 변환을 수행하기 때문에 해당 함수를 사용하여 특정 벡터를 지속적으로 움직인다고 해도
        /// 결국 Direction 벡터에 수렴하는 결과 값을 리턴하게 된다.
        ///
        /// 그 경우에는 Rotate함수를 사용하거나 혹은 Transform 없이 축회전이 가능한 VectorRotationUsingQuaternion 함수를 사용한다.
        /// 
        /// </summary>
        public static Vector3 VectorRotationUsingCrossingProduct(this Vector3 p_TargetUV, Vector3 p_DirectionUV, float p_Degree)
        {
            // 오른손 좌표계 기준 기저 벡터를 생성한다.
            var biasU = p_TargetUV;
            var biasW = Vector3.Cross(p_TargetUV, p_DirectionUV).normalized;
            var biasV = Vector3.Cross(biasW, biasU).normalized;
            
            // 삼각함수를 통해 각 기저 가중치를 선정해준다.
            var theta = Mathf.Deg2Rad * p_Degree;
            var sinTheta = Mathf.Sin(theta);
            var cosTheta = Mathf.Cos(theta);

            // UV 가중치 결합은 결과도 UV이므로 그대로 가중치 결합 값을 리턴해준다.
            return biasU * cosTheta + biasV * sinTheta;
        }

        /// <summary>
        /// 쿼터니언을 사용해서 벡터를 회전시키는 메서드
        /// 축회전을 사용하기 때문에, VectorRotationUsingCrossingProduct 처럼 기준 벡터에 의한
        /// 수렴현상이 발생하지 않는다.
        /// </summary>
        public static Vector3 VectorRotationUsingQuaternion(this Vector3 p_TargetUV, Vector3 p_PivotUV, float p_Degree)
        {
            return Quaternion.Euler(p_PivotUV * p_Degree) * p_TargetUV;
        }


        /// <summary>
        /// 특정 Transform이 지정한 Transform을 바라보도록 z축을 보간시키는 메서드
        /// </summary>
        public static void InterpolateDirection(this Transform p_TargetTransform, Transform p_LookAt, float p_Speed, float p_Dt)
        {
            Vector3 targetDir = p_LookAt.position - p_TargetTransform.position;
            float step = p_Speed * p_Dt;
            
            // 4번째 파라미터는 첫번째 파라미터와 두번째 파라미터의 magnitude가 다른 경우, 값을 같게 만들어주는데 더할 수 있는 실수 상한 값이다.
            // 0f이므로 당연히 magnitude 보정은 없다.
            Vector3 newDir = Vector3.RotateTowards(p_TargetTransform.forward, targetDir, step, 0f);

            p_TargetTransform.rotation = Quaternion.LookRotation(newDir);
        }

        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 다만, 해당 축벡터는 원점으로부터의 축벡터이다. 특정 오브젝트의 축을 기준으로 이동시키려면 동명의 다른 메서드를 사용할 것
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 RotationVectorByPivot(this Vector3 p_TargetVector3, Vector3 p_RotationPivotUnitVector, float p_Degree)
        {
            // 쿼터니언 자체가 오른손 좌표계를 쓰는 듯, 그래서 각도를 음수화 시켜준다.
            var halfTheta = Mathf.Deg2Rad * -p_Degree * 0.5f;
            var halfSin = Mathf.Sin(halfTheta);
            var halfCos = Mathf.Cos(halfTheta);
            
            var trailQuaternion = GetQuaternion(halfCos, -halfSin * p_RotationPivotUnitVector);
            var rearQuaternion = GetQuaternion(halfCos, halfSin * p_RotationPivotUnitVector);
            var targetQuaternion = GetQuaternion(0f, p_TargetVector3);

            return (trailQuaternion * targetQuaternion * rearQuaternion).TurnQuarternionVectorMode();
        }

        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 그러나 움직이는 회전체에 대해서는 역시 궤도 길이가 변한다는 한계점이 여전히 존재.
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotVector">회전할 회전축 중심 객체</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 RotationVectorByPivot(this Vector3 p_TargetVector3, Vector3 p_RotationPivotVector, Vector3 p_RotationPivotUnitVector, float p_Degree)
        {
            // 쿼터니언 자체가 오른손 좌표계를 쓰는 듯, 그래서 각도를 음수화 시켜준다.
            var halfTheta = Mathf.Deg2Rad * -p_Degree * 0.5f;
            var halfSin = Mathf.Sin(halfTheta);
            var halfCos = Mathf.Cos(halfTheta);
            var localTargetVector = p_TargetVector3 - p_RotationPivotVector;
            var trailQuaternion = GetQuaternion(halfCos, -halfSin * p_RotationPivotUnitVector);
            var targetQuaternion = GetQuaternion(0f, localTargetVector);
            var rearQuaternion = GetQuaternion(halfCos, halfSin * p_RotationPivotUnitVector);

            return (trailQuaternion * targetQuaternion * rearQuaternion).TurnQuarternionVectorMode() + p_RotationPivotVector;
        }
        
        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 그러나 움직이는 회전체에 대해서는 역시 궤도 길이가 변한다는 한계점이 여전히 존재.
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotTransform">회전할 회전축 중심 객체</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 RotationVectorByPivot(this Vector3 p_TargetVector3, Transform p_RotationPivotTransform, Vector3 p_RotationPivotUnitVector, float p_Degree)
        {
            return RotationVectorByPivot(p_TargetVector3, p_RotationPivotTransform.position, p_RotationPivotUnitVector, p_Degree);
        }
      
        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 그러나 회전축이 바뀌는 회전체에 대해서는 회전축과 각도가 바뀔수도 있다.
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotVector">회전할 회전축 중심 객체</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 RotationVectorByPivot(this Vector3 p_TargetVector3, Vector3 p_RotationPivotVector, Vector3 p_RotationPivotUnitVector, float p_Radius, float p_Degree)
        {
            // 쿼터니언 자체가 오른손 좌표계를 쓰는 듯, 그래서 각도를 음수화 시켜준다.
            var halfTheta = Mathf.Deg2Rad * -p_Degree * 0.5f;
            var halfSin = Mathf.Sin(halfTheta);
            var halfCos = Mathf.Cos(halfTheta);
            var localTargetVector = p_TargetVector3 - p_RotationPivotVector;
            var trailQuaternion = GetQuaternion(halfCos, -halfSin * p_RotationPivotUnitVector);
            var targetQuaternion = GetQuaternion(0f, localTargetVector);
            var rearQuaternion = GetQuaternion(halfCos, halfSin * p_RotationPivotUnitVector);

            return (trailQuaternion * targetQuaternion * rearQuaternion).TurnQuarternionVectorMode().normalized * p_Radius + p_RotationPivotVector;
        }
        
        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 그러나 회전축이 바뀌는 회전체에 대해서는 회전축과 각도가 바뀔수도 있다.
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotTransform">회전할 회전축 중심 객체</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 RotationVectorByPivot(this Vector3 p_TargetVector3, Transform p_RotationPivotTransform, Vector3 p_RotationPivotUnitVector, float p_Radius, float p_Degree)
        {
            return RotationVectorByPivot(p_TargetVector3, p_RotationPivotTransform.position, p_RotationPivotUnitVector, p_Radius, p_Degree);
        }
        
        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 값이 동적인 회전축에도 대응한다.
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotTransform">회전할 회전축 중심 객체</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 DynamicRotationVectorByPivot(this Vector3 p_TargetVector3, Vector3 p_RotationPivotTransform, Vector3 p_RotationPivotUnitVector, float p_Radius, float p_Degree)
        {
            // 쿼터니언 자체가 오른손 좌표계를 쓰는 듯, 그래서 각도를 음수화 시켜준다.
            var halfTheta = Mathf.Deg2Rad * -p_Degree * 0.5f;
            var halfSin = Mathf.Sin(halfTheta);
            var halfCos = Mathf.Cos(halfTheta);
            var localTargetVector = (p_TargetVector3 - p_RotationPivotTransform).GetPlaneProjectionVector(p_RotationPivotUnitVector);
            var trailQuaternion = GetQuaternion(halfCos, -halfSin * p_RotationPivotUnitVector);
            var targetQuaternion = GetQuaternion(0f, localTargetVector);
            var rearQuaternion = GetQuaternion(halfCos, halfSin * p_RotationPivotUnitVector);

            return (trailQuaternion * targetQuaternion * rearQuaternion).TurnQuarternionVectorMode().normalized * p_Radius + p_RotationPivotTransform;
        }
        
        /// <summary>
        /// 특정 좌표를, 지정한 회전 축벡터를 기준으로 왼손좌표계에서 봤을때 p_Degree 만큼 회전시킨 좌표를 리턴하는 메서드
        /// 값이 동적인 회전축에도 대응한다.
        /// </summary>
        /// <param name="p_TargetVector3">이동시킬 좌표</param>
        /// <param name="p_RotationPivotTransform">회전할 회전축 중심 객체</param>
        /// <param name="p_RotationPivotUnitVector">회전할 회전축 단위벡터</param>
        /// <param name="p_Degree">회전시킬 각도</param>
        public static Vector3 DynamicRotationVectorByPivot(this Vector3 p_TargetVector3, Transform p_RotationPivotTransform, Vector3 p_RotationPivotUnitVector, float p_Radius, float p_Degree)
        {
            return DynamicRotationVectorByPivot(p_TargetVector3, p_RotationPivotTransform.position, p_RotationPivotUnitVector, p_Radius, p_Degree);
        }
    }
}