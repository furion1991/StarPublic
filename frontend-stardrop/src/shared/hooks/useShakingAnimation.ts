'use client'

import { type RefObject, useEffect, useRef } from 'react'

import { getRandomIntFromInterval } from '@/shared/utils'

type UseShakingAnimationProps = {
  elementsRef: RefObject<HTMLElement | null>[]
  enabled: boolean
}

const ANIMATION_PARTS_NUMBER = 8

export const useShakingAnimation = ({ elementsRef, enabled }: UseShakingAnimationProps) => {
  const animationsRef = useRef<Animation[]>([])

  useEffect(() => {
    if (!enabled) {
      animationsRef.current.forEach((animation) => animation.cancel())
      animationsRef.current = []
      return
    }

    const newAnimations: Animation[] = []

    elementsRef.forEach(({ current: element }) => {
      if (!element) return

      const animation = element.animate(
        Array.from({ length: ANIMATION_PARTS_NUMBER }).map(() => ({
          transform: `translate(
            ${getRandomIntFromInterval(-10, 10)}px,
            ${getRandomIntFromInterval(-10, 10)}px
          )`
        })),
        {
          duration: 2000,
          iterations: Infinity,
          easing: 'ease-out',
          direction: 'alternate'
        }
      )

      newAnimations.push(animation)
    })

    animationsRef.current = newAnimations

    return () => {
      newAnimations.forEach((animation) => animation.cancel())
    }
  }, [enabled])

  useEffect(() => {
    if (!enabled) return

    const speedUpIntervalId = setInterval(() => {
      animationsRef.current.forEach((animation) => {
        animation.updatePlaybackRate(animation.playbackRate + 1)
      })
    }, 1000)

    return () => {
      clearInterval(speedUpIntervalId)
    }
  }, [enabled])
}
