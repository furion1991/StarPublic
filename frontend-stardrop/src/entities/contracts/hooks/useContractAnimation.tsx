'use client'

import { useState } from 'react'

const ANIMATION_DURATION_MS = 6000
const SPARKS_DELAY_MS = 4000

export const useContractAnimation = () => {
  const [isAnimationInProcess, setAnimationInProcess] = useState(false)
  const [isSparksActive, setSparksActive] = useState(false)

  const startAnimation = () => {
    // ANIMATION START
    // 1. shaking and glowing start
    setAnimationInProcess(true)

    // 2. two sec before end sparks active
    setTimeout(() => {
      setSparksActive(true)
    }, SPARKS_DELAY_MS)

    // 3. shaking, glowing, sparks end
    // ANIMATION END
    setTimeout(() => {
      setSparksActive(false)
      setAnimationInProcess(false)
    }, ANIMATION_DURATION_MS)
  }

  return {
    isAnimationInProcess,
    isSparksActive,
    startAnimation
  }
}
