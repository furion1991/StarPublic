'use client'

import { useState } from 'react'

const ANIMATION_DURATION_MS = 6000
const SPARKS_DELAY_MS = 4000
const EXPLOSION_DURATION_MS = 600

export const useUpgradeAnimation = () => {
  const [isAnimationInProcess, setAnimationInProcess] = useState(false)
  const [isShaking, setShaking] = useState(false)
  const [isGlowing, setGlowing] = useState(false)
  const [isExplosionVisible, setExplosionVisible] = useState(false)
  const [isSparksActive, setSparksActive] = useState(false)

  const startAnimation = () => {
    // ANIMATION START
    // 1. shaking and glowing start
    setAnimationInProcess(true)
    setShaking(true)
    setGlowing(true)

    // 2. two sec before end sparks active
    setTimeout(() => {
      setSparksActive(true)
    }, SPARKS_DELAY_MS)

    // 3. shaking, glowing, sparks end
    // 4. explosion start
    setTimeout(() => {
      setShaking(false)
      setExplosionVisible(true)
      setSparksActive(false)
    }, ANIMATION_DURATION_MS)

    // 5. explosion end
    // ANIMATION END
    setTimeout(() => {
      setGlowing(false)
      setExplosionVisible(false)
      setAnimationInProcess(false)
    }, ANIMATION_DURATION_MS + EXPLOSION_DURATION_MS)
  }

  return {
    isAnimationInProcess,
    isShaking,
    isGlowing,
    isSparksActive,
    isExplosionVisible,
    startAnimation
  }
}
