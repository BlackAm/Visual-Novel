namespace k514
{
    public class PetPetProjector : SimpleProjector
    {
        #region <Fields>
        
#if !SERVER_DRIVE
        private PetpetIterator _ScaleIterator;
#endif

        #endregion

        #region <Callbacks>

#if !SERVER_DRIVE
        public override void OnPooling()
        {
            base.OnPooling();
            _ScaleIterator.Reset();
        }

        protected override void OnLoadFirstImage()
        {
            var projectorExtraData = (PrefabExtraData_PetPetProjector.PrefabExtraDataPPProjectorRecord) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _AssetTuple = ImageNameTableData.GetInstanceUnSafe.GetTexture(projectorExtraData.ImageIndex,
                ResourceType.Image, ResourceLifeCycleType.Free_Condition);
            SetTexture(_AssetTuple.Item2);

            _ScaleIterator = new PetpetIterator(projectorExtraData.MinScale, projectorExtraData.MaxScale, projectorExtraData.Interval, projectorExtraData.LoopCount);
            SetScale(_ScaleIterator.GetCurrentScale());
        }

        protected override void OnFadeIn(float p_Dt)
        {
            base.OnFadeIn(p_Dt);
            OnUpdateScaleIterator(p_Dt);
        }

        protected override void OnProjection(float p_Dt)
        {
            base.OnProjection(p_Dt);
            OnUpdateScaleIterator(p_Dt);
        }

        protected override void OnFadeOut(float p_Dt)
        {
            base.OnFadeOut(p_Dt);
            OnUpdateScaleIterator(p_Dt);
        }

        private void OnUpdateScaleIterator(float p_Dt)
        {
            _ScaleIterator.ProgressIterating(p_Dt);
            SetScale(_ScaleIterator.GetCurrentScale());
        }
#endif
        
        #endregion
    }
}