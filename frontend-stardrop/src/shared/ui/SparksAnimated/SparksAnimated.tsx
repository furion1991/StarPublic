'use client'

import Image from 'next/image'
import { useEffect, useRef } from 'react'

import classes from './SparksAnimated.module.scss'

type SparksAnimatedProps = {
  active: boolean
}

export const SparksAnimated = ({ active }: SparksAnimatedProps) => {
  const animationsRef = useRef<Animation[]>([])

  const sparksRef1 = useRef<HTMLImageElement>(null)
  const sparksRef2 = useRef<HTMLImageElement>(null)
  const sparksRef3 = useRef<HTMLImageElement>(null)
  const sparksRef4 = useRef<HTMLImageElement>(null)
  const sparksRef5 = useRef<HTMLImageElement>(null)
  const sparksRef6 = useRef<HTMLImageElement>(null)
  const sparksRef7 = useRef<HTMLImageElement>(null)
  const sparksRef8 = useRef<HTMLImageElement>(null)

  const sparksGroupsImgProps = [
    {
      id: 0,
      ref: sparksRef1,
      src: '/img/upgrades/sparks/sparks-group-1.png',
      width: 609,
      height: 492
    },
    {
      id: 1,
      ref: sparksRef2,
      src: '/img/upgrades/sparks/sparks-group-2.png',
      width: 606,
      height: 575
    },
    {
      id: 2,
      ref: sparksRef3,
      src: '/img/upgrades/sparks/sparks-group-3.png',
      width: 581,
      height: 492
    },
    {
      id: 3,
      ref: sparksRef4,
      src: '/img/upgrades/sparks/sparks-group-4.png',
      width: 581,
      height: 522
    },
    {
      id: 4,
      ref: sparksRef5,
      src: '/img/upgrades/sparks/sparks-group-1.png',
      width: 609,
      height: 492
    },
    {
      id: 5,
      ref: sparksRef6,
      src: '/img/upgrades/sparks/sparks-group-2.png',
      width: 606,
      height: 575
    },
    {
      id: 6,
      ref: sparksRef7,
      src: '/img/upgrades/sparks/sparks-group-3.png',
      width: 581,
      height: 492
    },
    {
      id: 7,
      ref: sparksRef8,
      src: '/img/upgrades/sparks/sparks-group-4.png',
      width: 581,
      height: 522
    }
  ]

  useEffect(() => {
    if (!active) {
      animationsRef.current.forEach((animation) => animation.cancel())
      animationsRef.current = []
      return
    }

    const animations: Animation[] = []
    const elementsRef = [
      sparksRef1,
      sparksRef2,
      sparksRef3,
      sparksRef4,
      sparksRef5,
      sparksRef6,
      sparksRef7,
      sparksRef8
    ]

    elementsRef.forEach(({ current: element }, idx) => {
      if (!element) return

      const angle = idx * 45 * (Math.PI / 180)
      const distance = 500

      const x = Math.round(Math.sin(angle) * distance)
      const y = Math.round(-Math.cos(angle) * distance)

      const animation = element.animate(
        [
          {
            opacity: 1
          },
          {
            opacity: 0,
            transform: `translate(
              ${x}px, ${y}px
            )`
          }
        ],
        {
          duration: 450,
          easing: 'ease-out',
          iterations: Infinity
        }
      )

      animations.push(animation)
    })

    animationsRef.current = animations

    return () => {
      animations.forEach((animation) => animation.cancel())
    }
  }, [active])

  return (
    <div className={classes.sparksAnimated}>
      {sparksGroupsImgProps.map(({ id, ref, src, width, height }) => (
        <Image key={id} ref={ref} src={src} width={width} height={height} alt='Искры' />
      ))}
    </div>
  )
}
